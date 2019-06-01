using System.Collections;
using Photon.Pun;
using UnityEngine;

public partial class Skills
{
    // Actions ---------------------------------------------------------------------------------------------------------
    
    /// <summary>
    /// Tire sur la position donnee en parametre
    /// </summary>
    /// <param name="target"></param>
    public void Shoot(Vector3 target)
    {
        LookAt(target);

        //Calcule l'angle de tir en prenant en compte la gravite
        float launchSpeed = ballManager.GetLaunchSpeed();                     //La vitesse initiale
        float vSqr = launchSpeed * launchSpeed;                               //Au carre
        float g = Physics.gravity.magnitude;                                  //La gravite (9.81)
        float y = GetVerticalDistance(transform.position, target);            //La distance verticale entre la cible l'IA
        float x = GetHorizontalDistance(target, transform.position);          //La distance horizontale entre la cible l'IA

        float newPitch = -Mathf.Atan(
                             (vSqr - Mathf.Sqrt(
                                  vSqr * vSqr - g * (g * x * x + 2 * y * vSqr)
                              ))
                             / (g * x)
                         ) * 180 / Mathf.PI;

        if(!float.IsNaN(newPitch))
            SetPitch(newPitch);
        
        if(!shooting)
            StartCoroutine(ShootCoroutine(target));
    }
    
    /// <summary>
    /// Tire dans les cages
    /// </summary>
    public void Shoot()
    {
        Shoot(EnemyGoal().transform.position);
    }

    private bool shooting;
    IEnumerator ShootCoroutine(Vector3 target)
    {
        shooting = true;
        
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
        Shoot(PredictContactPoint(target, target.GetComponent<PlayerInfo>().velocity, ballManager.GetLaunchSpeed())); //Tire vers la cible en predisant sa trajectoire
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
            ballManager.Catch();
    }

    // Goals -----------------------------------------------------------------------------------------------------------
    
    public GameObject AllyGoal()
    {
        return GoalDetector.goals[infos.team == Team.Blue ? 0 : 1];
    }

    public GameObject EnemyGoal()
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
    
    public float HorizontalDistanceToBall()
    {
        return GetHorizontalDistance(Ball.ball.transform.position, infos.cameraPosition);
    }

    public float HorizontalDistanceFromGoalToBall()
    {
        return GetHorizontalDistance(Ball.ball.transform.position, AllyGoal().transform.position);
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

    public bool AllyPossessBall()
    {
        if (Ball.possessor == null)
            return false;
                
        return Ball.possessor.GetComponent<PlayerInfo>().team == infos.team;
    }
    
    public bool OpponnentPossessBall()
    {
        if (Ball.possessor == null)
            return false;
        
        return Ball.possessor.GetComponent<PlayerInfo>().team == infos.team.OtherTeam();
    }

    public bool InPositionToShoot()
    {
        return EnnemyGoalDist() <= Vector3.Distance(EnemyGoal().transform.position, OffensivePosition());
    }
}