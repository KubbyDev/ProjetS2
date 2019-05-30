using Photon.Pun;
using UnityEngine;

//Cette classe gere les strategies de l'IA, elle a acces a Skills qui est une list de competences de haut niveau
//Comme aller vers la balle, tirer, faire une passe, se demarquer etc

public class Brain : MonoBehaviour
{ 
    private Skills skills;       //Le script qui effectue les mouvement que ce script ordonne
    private PlayerInfo infos;    //Le script qui contient plein d'informations sur l'IA
    private Hero classe;
    private float WAYTOOCLOSEFROMGOAL = 5f;

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
        //MarkEnemy = 4,    //Idk about this one
        
        //Attack States
            // This has ball
        
            //Team has ball
        
        //Neutral
        GoToBall = 42,
    }

    public State currentState;
    
    void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        
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
            // Attaque
            
            //Autre
            case State.GoToBall:
            {
                skills.MoveTo(Ball.ball);
                break;
            }

        }
        
    }


    public State StateUpdate()
    {
        if (skills.AllyPossessBall() )     // Son equipe a la balle ils sont donc en position d'attaque
        {
            
        }

        else if (skills.OpponnentPossessBall()) // Ils sont en position de defense
        {
            if (skills.DistanceToBall() < BallNear)                            // Si la balle ets assez proche, on se dirige vers elle pour l'attraper
                return State.BallIsClose;

            if (skills.CanUseHook())                                           // Go hook si possible
                return State.Hooking;

            if (!skills.IsDefenderReady())                                    // S il n y a pas de defenseur de pret bah mdr c est la merde
                return State.ImminentDangerOMG;

            if (Vector3.Distance(Ball.ball.transform.position, skills.AllyGoal().transform.position) <= WAYTOOCLOSEFROMGOAL)
                return State.Defend;
            
        }

        // Dans le cas ou la balle est en l'air sans possesseur, le joueur le plus proche va vers celle-ci
        if (skills.GetNearestAllyFromBall().transform.position == infos.transform.position)
            return State.GoToBall;
        
        // Sinon, retour en defense
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
            skills.MoveTo(Ball.ball.transform.position);
        
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
}
