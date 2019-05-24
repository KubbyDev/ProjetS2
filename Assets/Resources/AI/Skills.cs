using System;
using System.Collections;
using System.Linq;
using Photon.Pun;
using UnityEngine;

//Le but de cette classe et de donner a Brain.cs l'acces a tout un tas de fonctionnalites
//de haut niveau (aller a un endroit, se demarquer, tirer, faire une passe, aller aux cages etc...)

public class Skills : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 540;
    
    public float timeToMove;          //Le temps restant avant que l'IA puisse bouger
    
    private Transform cam;            //Une fausse camera qui symbolise la direction du regard de l'IA  
    private MovementManager move;     //Reference au MovementManager de l'IA
    private BallManager ballManager;  //Reference au BallManager de l'IA
    private PlayerInfo infos;         //Reference au PlayerInfo de l'IA
    private Hook hook;                //Reference au gestionnaire du powerup hook
    private Back back;                //Reference au gestionnaire du powerup back
    private PowerShoot powerShoot;    //Reference au gestionnaire du powerup powershoot
    private Striker striker;          //Reference au script des spells du stricker
    private Warden warden;            //Reference au script des spells du warden
    private Ninja ninja;              //Reference au script des spells du ninja

    private Vector3 targetPosition;
    private Quaternion targetRotation;
    
    void Start()
    {
        move = GetComponent<MovementManager>();
        ballManager = GetComponent<BallManager>();
        infos = GetComponent<PlayerInfo>();
        cam = transform.Find("CameraAnchor");
        hook = GetComponent<Hook>();
        back = GetComponent<Back>();
        powerShoot = GetComponent<PowerShoot>();
        striker = GetComponent<Striker>();
        warden = GetComponent<Warden>();
        ninja = GetComponent<Ninja>();
    }

    void Update()
    {
        if (timeToMove > 0)
        {
            //Met a jour le temps restant pour pouvoir bouger
            timeToMove -= Time.deltaTime;
        }
        else
        {
            //Se deplace vers targetPosition
            Vector3 moveInput = targetPosition - transform.position;
            moveInput.y = 0;
            move.Move(moveInput.normalized);   
        }

        //Se tourne vers targetRotation
        cam.rotation = Quaternion.RotateTowards(cam.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(new Vector3(0, cam.rotation.eulerAngles.y, 0));
        
        infos.cameraRotation = cam.rotation;
        infos.cameraPosition = cam.position;
    }

    // Competences -----------------------------------------------------------------------------------------------------

    /// <summary>
    /// Se dirige entre le but et le joueur le plus proche du but (en regardant le but adverse)
    /// </summary>
    public void MoveToSupportPosition()
    {
        MoveTo((GetNearestOpponentFromGoal().transform.position + GetAllyGoal().transform.position)/2, false);
    }

    //Essaye d'attraper la balle
    public void CatchBall()
    {
        LookAt(Ball.ball.transform.position);
        
        //Un seul client a le droit de demander a l'IA d'attraper la balle
        //Si un client a la balle, c'est lui, sinon c'est le host
        if(Ball.possessor != null ? PlayerInfo.localPlayer == Ball.possessor : PhotonNetwork.IsMasterClient)    
            ballManager.Catch();
    }
    
    private bool shooting;
    /// <summary>
    /// Tire sur la position donnee en parametre
    /// </summary>
    /// <param name="target"></param>
    public void Shoot(Vector3 target)
    {
        if(!shooting)
            StartCoroutine(ShootCoroutine(target));
    }

    IEnumerator ShootCoroutine(Vector3 target)
    {
        shooting = true; 
        LookAt(target);

        //Calcule l'angle de tir en prenant en compte la gravite
        float launchSpeed = ballManager.GetLaunchSpeed();                     //La vitesse initiale
        float vSqr = launchSpeed * launchSpeed;                               //Au carre
        float g = Physics.gravity.magnitude;                                  //La gravite (9.81)
        float y = target.y - transform.position.y;                            //La distance verticale entre la cible l'IA
        float x = (target-transform.position-y*Vector3.up).magnitude;         //La distance horizontale entre la cible l'IA

        float newPitch = -Mathf.Atan(
                             (vSqr - Mathf.Sqrt(
                                  vSqr * vSqr - g * (g * x * x + 2 * y * vSqr)
                             ))
                             / (g * x)
                         ) * 180 / Mathf.PI;

        if(!float.IsNaN(newPitch))
            SetPitch(newPitch);

        //Attend que la balle arrete de bouger avant de tirer (elle bouge parce qu'il vient de se tourner)
        yield return new WaitForSeconds(0.5f);

        ballManager.Shoot();       
        shooting = false;
    }

    /// <summary>
    /// Tire dans les cages
    /// </summary>
    public void Shoot()
    {
        Shoot(GetEnemyGoal().transform.position);
    }

    /// <summary>
    /// Fait une passe
    /// </summary>
    /// <param name="target"></param>
    public void Pass(GameObject target)                
    {
        Shoot(target.transform.position + //Tire vers la cible en predisant sa trajectoire
              target.GetComponent<PlayerInfo>().velocity 
              * Vector3.Distance(target.transform.position, transform.position)
              / ballManager.GetLaunchSpeed());
              //Le vecteur a ajouter pour faire la predition est v_target * d / v_balle (c'est pas parfait mais c'est suffisant)
    }
    
    // Spells et PowerUps ----------------------------------------------------------------------------------------------

    public void UseTurbo()
    {
        if(infos.hero == Hero.Stricker)
            striker.Speed();
        else
            Debug.Log("UseTurbo appellee sur un " + infos.hero);
    }
    
    public void UseEscape()
    {
        if(infos.hero == Hero.Stricker)
            striker.Escape();
        else
            Debug.Log("UseEscape appellee sur un " + infos.hero);
    }
    
    public void UseMagnet()
    {
        if(infos.hero == Hero.Warden)
            warden.MagnetSpell();
        else
            Debug.Log("UseMagnet appellee sur un " + infos.hero);
    }
    
    public void UseFreeze()
    {
        if(infos.hero == Hero.Warden)
            warden.Freeze();
        else
            Debug.Log("UseFreeze appellee sur un " + infos.hero);
    }

    public void UseExplode()
    {
        if(infos.hero == Hero.Ninja)
            ninja.Explode_Spell();
        else
            Debug.Log("UseExplode appellee sur un " + infos.hero);
    }

    public void UseSmoke()
    {
        if(infos.hero == Hero.Ninja)
            ninja.Smoke();
        else
            Debug.Log("UseSmoke appellee sur un " + infos.hero);
    }

    public void UseHook()
    {
        hook.Use_Hook();
    }

    public void UseBack()
    {
        back.TP_Back();
    }

    public void UsePowerShoot()
    {
        powerShoot.Use_PowerShoot();
    }

    // Fonctions basiques ----------------------------------------------------------------------------------------------
    
    /// <summary>
    /// Tourne sur l'axe horinzontal (angle en degres)
    /// </summary>
    /// <param name="yaw"></param>
    public void SetYaw(float yaw)
    {
        SetOrientation(targetRotation.eulerAngles.x, yaw);
    }
    
    /// <summary>
    /// Tourne sur l'axe vertical (angle en degres)
    /// </summary>
    /// <param name="pitch"></param>
    public void SetPitch(float pitch)
    {
        SetOrientation(pitch, targetRotation.eulerAngles.y);
    }

    /// <summary>
    /// Tourne l'IA (pitch = axe vertical, yaw = axe horinzontal)
    /// </summary>
    /// <param name="pitch"></param>
    /// <param name="yaw"></param>
    public void SetOrientation(float pitch, float yaw)
    {
        targetRotation = Quaternion.Euler(new Vector3(pitch, yaw, 0));
    }
    
    /// <summary>
    /// Regarde le point specifie
    /// </summary>
    /// <param name="point"></param>
    public void LookAt(Vector3 point)
    {
        targetRotation = Quaternion.LookRotation(point - transform.position);
    }

    public void Jump()
    {
        if(timeToMove <= 0)
            move.Jump();
    }
    
    /// <summary>
    /// Bouge jusqu'a position. UpdateRotation = true => regarde devant lui en marchant
    /// </summary>
    public void MoveTo(Vector3 position, bool updateRotation = true)
    {
        if(updateRotation)
            LookAt(position);

        targetPosition = position;
    }
    
    // Getters ---------------------------------------------------------------------------------------------------------

    public bool CanUseBasicAttack()
    {
        return infos.BACooldown <= 0;
    }
    
    public bool CanUseFirstSpell()
    {
        return infos.firstCooldown <= 0;
    }
    
    public bool CanUseSecondSpell()
    {
        return infos.secondCooldown <= 0;
    }

    public bool HasHook()
    {
        return hook.Player_Has_Hook;
    }
    
    public bool HasBack()
    {
        return back.Player_Has_Back;
    }
    
    public bool HasPowerShoot()
    {
        return powerShoot.Player_Has_PowerShoot;
    }
    
    public bool HasBall()
    {
        return ballManager.hasBall;
    }

    public float DistanceToBall()
    {
        return Vector3.Distance(Ball.ball.transform.position, transform.position);
    }

    public GameObject GetAllyGoal()
    {
        return GoalDetector.goals[infos.team == Team.Blue ? 0 : 1];
    }

    public GameObject GetEnemyGoal()
    {
        return GoalDetector.goals[infos.team == Team.Blue ? 1 : 0];
    }

    /// <summary>
    /// Renvoie le joueur le plus proche de fromPosition parmis ceux qui satisfont la condition filter
    /// </summary>
    /// <param name="filter"></param>
    /// <param name="fromPosition"></param>
    /// <returns></returns>
    public GameObject GetNearestPlayer(Func<GameObject, bool> filter, Vector3 fromPosition)
    {
        float minDist = 0;
        GameObject nearest = null;

        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player").Where(filter))
        {
            float dist = Vector3.Distance(player.transform.position, fromPosition);
            if (dist < minDist)
            {
                nearest = player;
                minDist = dist;
            }
        }

        return nearest;
    }

    /// <summary>
    /// Renvoie l'adversaire le plus proche
    /// </summary>
    /// <returns></returns>
    public GameObject GetNearestOpponent()
    {
        return GetNearestPlayer(
            player => player.GetComponent<PlayerInfo>().team.IsOpponnentOf(infos.team),
            transform.position
        );  
    }

    /// <summary>
    /// Renvoie l'adversaire le plus proche des buts allies
    /// </summary>
    /// <returns></returns>
    public GameObject GetNearestOpponentFromGoal()
    {
        return GetNearestPlayer(
            player => player.GetComponent<PlayerInfo>().team.IsOpponnentOf(infos.team),
            GetAllyGoal().transform.position
        );  
    }
}
