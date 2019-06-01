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
    private float WAYTOOCLOSEFROMGOAL = 25f;
    private float CloseEnoughToShoot = 20f;
    private float PUveryclose = 5f;
    private float PUprettyclose = 10f;

    // Setup des infos importantes -------------------------------------------------------------------------------------
    
    void Start()
    {
        skills = GetComponent<Skills>();
        infos = GetComponent<PlayerInfo>();
        classe = infos.hero;
    }

    // Strategies ------------------------------------------------------------------------------------------------------
    
    private float BallNear = 13f;  // Distance a partir de laquelle on considere que la balle est assez proche pour qu on aille la chercher (a ajuster)
    
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
                Shoot();
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
        if (skills.GetNearestAllyFromBall().GetComponent<PlayerInfo>() == infos &&
            skills.GetNearestAllyFromAllyGoal().GetComponent<PlayerInfo>() != infos)
            return State.GoToBall;
        
        if (skills.AllyPossessBall()) // Son equipe a la balle ils sont donc en position d'attaque
            return AttackStateUpdate();

        if (skills.OpponnentPossessBall()) // Ils sont en position de defense
            return DefendStateUpdate();

        // Dans le cas ou la balle est en l'air sans possesseur, le joueur le plus proche va vers celle-ci
        if (skills.GetNearestAllyFromBall().GetComponent<PlayerInfo>() == infos)
            return State.GoToBall;

        // Si le joueur est le plus proche des cages, il va en defense
        if (skills.GetNearestAllyFromAllyGoal().GetComponent<PlayerInfo>() == infos)
            return State.GoBackToDef;
        
        return State.GoSupport;
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
            
        if (skills.GetNearestAllyFromAllyGoal() != infos.gameObject)
            return State.MarkEnemy;

        return State.GoBackToDef;
    }

    public State AttackStateUpdate()
    {
        if (skills.HasBall())
            return ThisGotBall();

        return AlliesGotBall();
        
    }

    public State ThisGotBall()
    {
        if (!skills.IsDefenderReady())
            return State.Shoot;
        
        if (!skills.IsFree(infos.gameObject))
            return State.Pass;
        
        if (skills.InPositionToShoot())
            return State.Shoot;
        
        if (infos.hero == Hero.Warden && skills.GetNearestFreeAlly())
            return State.Pass;
        if (skills.EnnemyGoalDist() >= Vector3.Distance(skills.OffensivePosition(), skills.EnemyGoal().transform.position))
            return State.Shoot;
        if (skills.IsFree(infos.gameObject))
            return State.MoveForth;
        return State.Pass;
    }

    public State AlliesGotBall()
    {
        if (infos.hero != Hero.Warden)
        {
            //////// Inserer partie qvec recuperqtion des powerups
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

        if (skills.IsBallCloseEnough())
        {
            skills.LookAt(Ball.ball.transform.position);
            skills.CatchBall();
        }
        
        else
            skills.MoveTo(Ball.ball, true); 
        
    }

    public void Hook() => skills.UseHookSmartly();
    

    public void GoBackToDef()
    {
        
        if(!skills.HasBack())
        {
            var PU = skills.GetNearestPowerUp(PowerUp.Back);
            if (PU != null && Vector3.Distance(infos.gameObject.transform.position, PU.transform.position) < skills.AllyGoalDist())
            skills.MoveTo(PU);
        }
        if (skills.HasBack())
            skills.UseBack();


        else if (classe == Hero.Stricker)
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
        if (skills.GetNearestAllyFromAllyGoal() == infos.gameObject && Vector3.Distance(skills.GetNearestOpponentFromBall().transform.position, skills.AllyGoal().transform.position) < WAYTOOCLOSEFROMGOAL )
            skills.MoveInGoal();
        
        if (!skills.HasHook())
        {
            var PU = skills.GetNearestPowerUp(PowerUp.Hook);
            if (PU!=null && Vector3.Distance(PU.transform.position, infos.gameObject.transform.position) <= PUveryclose) 
                skills.MoveTo(PU);
        }

        if (skills.CanUseHook())
            skills.UseHookSmartly();

        else
            GoToBall();
        
    }

    public void GoToBall()
    {
        if (skills.DistanceToBall() > infos.maxCatchRange + 2f && Ball.possessor!= null )
            skills.MoveTo(Ball.possessor);
        else
            skills.MoveTo(Ball.ball, true);
        skills.CatchBall();
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
        skills.MoveToAvoidingEnnemies(skills.OffensivePosition());
    }
    
    public void MarkEnnemy()
    {
        var target = skills.GetNearestPlayer(
            player => player.GetComponent<PlayerInfo>().team.IsOpponnentOf(infos.team) && skills.IsFree(player),
            transform.position,
            true);
        if (target != null)
            skills.MoveTo(target.transform.position);
        else
            skills.MoveTo(skills.SupportPosition());
    }

    public void GoSupport()
    {
        if (Ball.possessor!= null && skills.IsFree(Ball.possessor) && Ball.possessor.GetComponent<PlayerInfo>().team == infos.team)
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

            else
                skills.MoveToOffensivePosition();
        }
    }

    public void Pass()
    {
        GameObject allytopass = skills.GetNearestAllyInFront();
        if (allytopass == null)
            allytopass = skills.GetNearestFreeAlly();
        if (allytopass != null) 
            skills.Pass(allytopass);
        else
            skills.Shoot();
        
        
    }

    public void Shoot()
    {
        if (!skills.HasPowerShoot())
        {
            var PU = skills.GetNearestPowerUp(PowerUp.PowerShoot);
            if (PU!=null && Vector3.Distance(PU.transform.position, infos.gameObject.transform.position) <= PUveryclose) 
                skills.MoveTo(PU);
        }

        if (skills.HasPowerShoot())
            skills.UsePowerShoot();
        
       skills.Shoot();
    }
}
