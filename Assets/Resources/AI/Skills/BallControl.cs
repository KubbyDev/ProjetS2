using System.Collections;
using Photon.Pun;
using UnityEngine;

public partial class Skills
{
    private bool shooting;
    
    // Actions ---------------------------------------------------------------------------------------------------------
    
    /// <summary>
    /// Tire sur la position donnee en parametre
    /// </summary>
    /// <param name="target"></param>
    public void Shoot(Vector3 target)
    {
        if(!shooting)
            StartCoroutine(ShootCoroutine(target));
    }
    
    /// <summary>
    /// Tire dans les cages
    /// </summary>
    public void Shoot()
    {
        Shoot(GetEnemyGoal().transform.position);
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
    
    /// <summary>
    /// Essaye d'attraper la balle (Il faut spammer cette fonction
    /// parce que si la balle n'est pas devant les yeux de l'IA,
    /// cette fonction se contente de bouger la camera vers la balle)
    /// Utiliser IsBallCloseEnough pour savoir si la balle est a portee
    /// </summary>
    public void CatchBall()
    {
        if (!IsBallCloseEnough())
            return;
        
        if(!CanCatchBall())
            LookAt(Ball.ball.transform.position);
        else
        {
            //Un seul client a le droit de demander a l'IA d'attraper la balle
            //Si un client a la balle, c'est lui, sinon c'est le host
            if(Ball.possessor != null ? PlayerInfo.localPlayer == Ball.possessor : PhotonNetwork.IsMasterClient)    
                ballManager.Catch();   
        }
    }
    
    // Goals -----------------------------------------------------------------------------------------------------------
    
    public GameObject GetAllyGoal()
    {
        return GoalDetector.goals[infos.team == Team.Blue ? 0 : 1];
    }

    public GameObject GetEnemyGoal()
    {
        return GoalDetector.goals[infos.team == Team.Blue ? 1 : 0];
    }
    
    // Getters ---------------------------------------------------------------------------------------------------------
    
    public bool HasBall()
    {
        return ballManager.hasBall;
    }

    public float DistanceToBall()
    {
        return Vector3.Distance(Ball.ball.transform.position, infos.cameraPosition);
    }
    
    /// <summary>
    /// Prend en compte les spells comme le magnet
    /// </summary>
    /// <returns></returns>
    public bool IsBallCloseEnough()
    {
        return DistanceToBall() < infos.maxCatchRange;
    }

    /// <summary>
    /// Renvoie vrai si un clic suffirait pour attraper la balle
    /// </summary>
    /// <returns></returns>
    public bool CanCatchBall()
    {
        return IsBallCloseEnough() && ballManager.CanCatch();
    }
}