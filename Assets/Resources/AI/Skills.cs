using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

//Le but de cette classe et de donner a Brain.cs l'acces a tout un tas de fonctionnalites
//de haut niveau (aller a un endroit, se demarquer, tirer, faire une passe, aller aux cages etc...)

public class Skills : MonoBehaviour
{
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
        infos.cameraRotation = cam.rotation;
        infos.cameraPosition = cam.position;
        
        //Met a jour le temps restant pour pouvoir bouger
        if (timeToMove > 0)
            timeToMove -= Time.deltaTime;
    }

    // Competences -----------------------------------------------------------------------------------------------------

    private bool shooting;
    
    //Bouger jusqu'a position
    public void MoveTo(Vector3 position)
    {
        LookAt(position);
        Vector3 moveInput = position - transform.position;
        moveInput.y = 0;
        Move(moveInput.normalized);
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

    /// <summary>
    /// Tire sur la position donnee en parametre
    /// </summary>
    /// <param name="targetPosition"></param>
    public void Shoot(Vector3 targetPosition)
    {
        if(!shooting)
            StartCoroutine(ShootCoroutine(targetPosition));
    }

    IEnumerator ShootCoroutine(Vector3 targetPosition)
    {
        shooting = true; 
        LookAt(targetPosition);
        
        //Attend que la balle arrete de bouger avant de tirer (elle bouge parce qu'il vient de se tourner)
        yield return new WaitForSeconds(0.3f);
        
        //Calcule l'angle de tir en prenant en compte la gravite
        float launchSpeed = ballManager.GetLaunchSpeed();                     //La vitesse initiale
        float vSqr = launchSpeed * launchSpeed;                               //Au carre
        float g = Physics.gravity.magnitude;                                  //La gravite (9.81)
        float y = targetPosition.y - transform.position.y;                    //La distance verticale entre la cible l'IA
        float x = (targetPosition-transform.position-y*Vector3.up).magnitude; //La distance horizontale entre la cible l'IA

        float newPitch = -Mathf.Atan(
                             (vSqr - Mathf.Sqrt(
                                  vSqr * vSqr - g * (g * x * x + 2 * y * vSqr)
                             ))
                             / (g * x)
                         ) * 180 / Mathf.PI;
        
        if(!float.IsNaN(newPitch))
            SetPitch(newPitch);
        
        ballManager.Shoot();       
        shooting = false;
    }

    /// <summary>
    /// Tire dans les cages
    /// </summary>
    public void Shoot()
    {
        Shoot(GoalDetector.goals[infos.team == Team.Blue ? 1 : 0].transform.position);
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
    
    public void Turn(float newRotation)
    {
        transform.eulerAngles = new Vector3(0, newRotation, 0);
        
        infos.cameraRotation = cam.rotation;
    }
    
    private void SetPitch(float pitch)
    {
        cam.eulerAngles = new Vector3(pitch, cam.eulerAngles.y, 0);
        
        infos.cameraRotation = cam.rotation;
    }
    
    //Regarde le point specifie
    public void LookAt(Vector3 point)
    {
        cam.LookAt(point);
        transform.eulerAngles = new Vector3(0, cam.rotation.eulerAngles.y, 0);
        cam.eulerAngles = new Vector3(cam.eulerAngles.x, transform.eulerAngles.y, 0);
        
        infos.cameraRotation = cam.rotation;
    }

    public void Jump()
    {
        move.Jump();
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
    
    // Fonctions annexes -----------------------------------------------------------------------------------------------
    
    private void Move(Vector3 input)
    {
        if(timeToMove <= 0)
            move.Move(input);
    }
}
