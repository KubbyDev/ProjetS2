using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

//Cette classe gere les strategies de l'IA, elle a acces a Skills qui est une list de competences de haut niveau
//Comme aller vers la balle, tirer, faire une passe, se demarquer etc

public class Brain : MonoBehaviour
{ 
    private Skills skills;       //Le script qui effectue les mouvement que ce script ordonne
    private PlayerInfo infos;    //Le script qui contient plein d'informations sur l'IA
    private Hero classe;
    private float WAYTOOCLOSEFROMGOAL = 3f;
    private float CloseEnoughToShoot = 10f;

    // Setup des infos importantes -------------------------------------------------------------------------------------
    
    void Start()
    {
        skills = GetComponent<Skills>();
        infos = GetComponent<PlayerInfo>();
        classe = infos.hero;
    }

    // Strategies ------------------------------------------------------------------------------------------------------
    
    private int BallNear = 20;  // Distance a partir de laquelle on considere que la balle est assez proche pour qu on aille la chercher (a ajuster)
    
    public enum State
    {
        None = 0,
        
        //Defense States
        BallIsClose = 1,
        ImminentDangerOMG = 2,
        GoBackToDef = 3,
        Defend = 4,
        Hooking = 5,
        MarkEnemy = 6,   
        
        //Attack States
            // This has ball
            
        Shoot = 51,
        MoveForth = 52,
        Pass = 53,
        GoSupport = 54,
            //Team has ball
        
        //Neutral
        GoToBall = 42,
    }

    public State currentState;
    
    void Update()
    {
        //Fait en sorte que ce soit toujours l'hote qui gere les IA
        if (!PhotonNetwork.IsMasterClient)
            return;
        else
            if (GetComponent<PhotonView>().Owner != PhotonNetwork.MasterClient)
                GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.MasterClient);
                
        //Determiner le state
        currentState = StateUpdate();
        
        switch (currentState)
        {

            //DEFENSE
            case State.BallIsClose:
            {
                BallIsClose();
                break;
            }

            case State.ImminentDangerOMG:
            {
                ImminentDangerONG();
                break;
            }

            case State.GoBackToDef:
            {
                GoBackToDef();
                break;
            }
            
            case State.Defend:
            {
                Defend();
                break;
            }

            case State.Hooking:
            {
                Hook();
                break;
            }

            case State.MarkEnemy:
            {
                MarkEnnemy();
                break;
            }
            
            // Attaque
            case State.Shoot:
            {
                skills.Shoot();
                break;
            }

            case State.MoveForth:
            {
                MoveForth();
                break;
            }
            
            case State.GoSupport:
            {
                GoSupport();
                break;
            }

            case State.Pass:
            {
                Pass();
                break;
            }
            
            //Autre
            case State.GoToBall:
            {
                GoToBall();
                break;
            }

            
        }
        
    }


    public State StateUpdate()
    {
        if (skills.AllyPossessBall()) // Son equipe a la balle ils sont donc en position d'attaque
            return AttackStateUpdate();

        if (skills.OpponnentPossessBall()) // Ils sont en position de defense
            return DefendStateUpdate();

        // Dans le cas ou la balle est en l'air sans possesseur, le joueur le plus proche va vers celle-ci
        if (skills.GetNearestAllyFromBall().transform.position == infos.transform.position)
            return State.GoToBall;
        
        // Sinon, retour en defense (par defaut)
        return State.GoBackToDef;
    }

    private State DefendStateUpdate()
    {
        if (skills.DistanceToBall() < BallNear)                            // Si la balle ets assez proche, on se dirige vers elle pour l'attraper
            return State.BallIsClose;

        if (skills.CanUseHook())                                           // Go hook si possible
            return State.Hooking;

        if (skills.GetNearestPlayerFromAllyGoal().GetComponent<PlayerInfo>().team.IsOpponnentOf(infos.team))                                    // S il n y a pas de defenseur allie entre le joueur ennemi et les cages bah mdr c est la merde
            return State.ImminentDangerOMG;

        if (Vector3.Distance(Ball.ball.transform.position, skills.AllyGoal().transform.position) <= WAYTOOCLOSEFROMGOAL)
            return State.Defend;
            
        if (skills.GetNearestAllyFromAllyGoal() != infos.gameObject && skills.GetNearestFreeEnemy())
            return State.MarkEnemy;

        return State.GoBackToDef;
    }

    public State AttackStateUpdate()
    {
        if (Ball.possessor == infos.gameObject)
            return ThisGotBall();

        return AlliesGotBall();
        
    }

    public State ThisGotBall()
    {
        if (infos.hero == Hero.Warden && skills.GetNearestFreeAlly())
            return State.Pass;
        if (skills.EnnemyGoalDist() <= CloseEnoughToShoot)
            return State.Shoot;
        if (skills.IsFree(infos.gameObject))
            return State.MoveForth;
        return State.Pass;
    }

    public State AlliesGotBall()
    {
        if (infos.hero != Hero.Warden)
        {
            if (skills.GetNearestAllyFromAllyGoal() != infos.gameObject)
                return State.GoSupport;
        }

        if (skills.GetNearestAllyFromAllyGoal() != infos.gameObject)
        {
            return State.GoSupport;
        }
        return State.GoBackToDef;
    }


    public void BallIsClose()                            //Quand la balle est proche, va vers la balle / essaie de la recuperer
    {
        if (classe == Hero.Warden && skills.CanUseMagnet() )
            skills.UseMagnet();

        if (skills.CanCatchBall())
            skills.CatchBall();
        
        else
            skills.MoveTo(Ball.ball, true); 
        
    }

    public void Hook() => skills.UseHookSmartly();
    

    public void GoBackToDef()
    {
        if (skills.HasBack())
            skills.UseBack();
        

        if (classe == Hero.Stricker)
        {
            if ( skills.CanUseSecondSpell())
                skills.UseEscapeSmartly(skills.AllyGoal().transform.position);
            if (skills.CanUseFirstSpell())
                skills.UseTurbo();
        }
        
        skills.MoveToDefensivePosition();
    }

    public void Defend()
    {
        if (classe == Hero.Warden && skills.CanUseFreeze())
            skills.UseFreezeSmartly();
        if (classe == Hero.Ninja && skills.CanUseFirstSpell())
            skills.UseExplodeSmartly(skills.GetNearestOpponentFromBall());
        if (Vector3.Distance(skills.GetNearestOpponentFromBall().transform.position, skills.AllyGoal().transform.position) < WAYTOOCLOSEFROMGOAL )
            skills.MoveInGoal();
        else
            GoToBall();
        
    }

    public void GoToBall()
    {
        if (skills.DistanceToBall() > infos.maxCatchRange + 2f && Ball.possessor!=null)
            skills.MoveTo(Ball.possessor);
        else
            skills.MoveTo(Ball.ball, true);
    }   

    public void ImminentDangerONG()
    {
        if (skills.GetNearestOpponentFromAllyGoal() == skills.GetNearestOpponentFromBall())
        {
            if (classe == Hero.Stricker && skills.CanUseSecondSpell())
                skills.UseEscapeSmartly(skills.SupportPosition());
            
            skills.MoveToSupportPosition();
        }
        skills.MoveInGoal();
    }


    public void MoveForth()
    {
        if (infos.hero == Hero.Stricker)
        {
            if (skills.CanUseSecondSpell())
                skills.UseEscapeSmartly(skills.EnemyGoal().transform.position);
            if (skills.CanUseFirstSpell())
                skills.UseTurbo();
           
        }

        if (infos.hero == Hero.Ninja)
        {
            if (skills.CanUseSecondSpell())
                skills.UseSmoke();
            if (skills.CanUseFirstSpell() && skills.IsDefenderReady())
                skills.UseExplodeSmartly(skills.GetNearestOpponentFromEnemyGoal());
            
        }
        skills.MoveToGoalAvoidingEnnemies();
    }
    public void MarkEnnemy()
    {
        skills.MoveTo(skills.GetNearestPlayer(
            player => player.GetComponent<PlayerInfo>().team != infos.team && skills.IsFree(player),
            transform.position,
            true).transform.position);
    }

    public void GoSupport()
    {
        if (skills.IsFree(Ball.possessor))
        {
            if (infos.hero == Hero.Stricker && skills.CanUseFirstSpell())            // On garde a TP en cas de contre attaque rapide
                    skills.UseTurbo();
            
            if (infos.hero == Hero.Ninja )                                           // Le Ninja investit tout ce qu'il a pour aider l'attaquant
            {
                if (skills.CanUseFirstSpell() && skills.IsDefenderReady())
                    skills.UseExplodeSmartly(skills.GetNearestOpponentFromEnemyGoal());
                if (skills.CanUseSecondSpell())
                {
                    skills.LookAt(skills.GetNearestOpponentFromBall().transform.position);
                    skills.UseSmoke();
                }
            }
            
            skills.MoveToGoalAvoidingEnnemies();
        }
    }

    public void Pass()
    {
        GameObject allytopass = skills.GetNearestAllyInFront();
        if (allytopass == null)
            allytopass = skills.GetNearestFreeAlly();
        if (allytopass == null)
            skills.MoveToAvoidingEnnemies(skills.AllyGoal().transform.position);
        else
            skills.Pass(allytopass);
    }
}
