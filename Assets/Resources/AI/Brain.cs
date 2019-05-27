using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine;

//Cette classe gere les strategies de l'IA, elle a acces a Skills qui est une list de competences de haut niveau
//Comme aller vers la balle, tirer, faire une passe, se demarquer etc

public class Brain : MonoBehaviour
{ 
    private Skills skills;       //Le script qui effectue les mouvement que ce script ordonne
    private PlayerInfo infos;    //Le script qui contient plein d'informations sur l'IA
    private Ball ball;

    // Setup des infos importantes -------------------------------------------------------------------------------------
    
    void Start()
    {
        skills = GetComponent<Skills>();
        infos = GetComponent<PlayerInfo>();
        ball = GetComponent<Ball>();
    }

    // Strategies ------------------------------------------------------------------------------------------------------
    
    public enum State
    {
        None = 0,
        //Defense States
        BallIsClose = 1,
        EnnemyIsClose = 2,
        EnemyIsTooFar = 3,
        GoBackToDef = 4
        //MarkEnemy = 4,    //Idk about this one
        
        //Attack States
            // This has ball
        
            //Team has ball
        
    }

    public State currentState;
    
    void Update()
    {
        //Determiner le state
        currentState = StateUpdate();
        switch (currentState)
        {
            case State.BallIsClose:
            {
                BallIsClose();
                break;
            }
        }
        
        
        
    }

        if(skills.DistanceToBall() < 50)
            skills.UseFreezeSmartly();

    public State StateUpdate()
    {
        if (ball.lastTeamIsBlue && infos.team == Team.Blue)     // Son equipe a la balle ils sont donc en position d'attaque
        {
            
        }

        else                                                    // Ils sont en position de defense
        {
            if (skills.IsBallClose())                            // Si la balle ets assez proche, on se dirige vers elle pour l'attraper
                return State.BallIsClose;
            // Inserer la partie avec le HOOK

            if (skills.AllyGoalDist()>= skills.GoalsDist()*2/3)
            {
                return State.GoBackToDef;
            }
            
        }

        return State.None;
    }


    public void BallIsClose()                            //Quand la balle est proche
    {
        if(skills.HasBall())
            skills.Shoot();
        else
        {
            skills.MoveToDefensivePosition();
                
            if(skills.HorizontalDistanceFromGoalToBall() < 50)
                skills.MoveTo(Ball.ball, true); 
                
            skills.CatchBall();   
        }
        */
    }
}
