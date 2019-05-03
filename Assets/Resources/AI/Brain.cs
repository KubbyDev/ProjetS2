using UnityEngine;

//Cette classe gere les strategies de l'IA, elle a acces a Skills sui est une list de competences de haut niveau
//Comme aller vers la balle, tirer, faire une passe, se demarquer etc

public class Brain : MonoBehaviour
{ 
    private Skills skills;       //Le script qui effectue les mouvement que ce script ordonne
    private PlayerInfo infos;    //Le script qui contient plein d'informations sur l'IA

    // Setup des infos importantes -------------------------------------------------------------------------------------
    
    void Start()
    {
        skills = GetComponent<Skills>();
        infos = GetComponent<PlayerInfo>();
    }

    // Strategies ------------------------------------------------------------------------------------------------------
    
    public enum State
    {
        GoToTheBall = 1
    }

    public State currentState;
    
    void Update()
    {
        // A gnee gneee taper ballon
        currentState = State.GoToTheBall;
        
        if (currentState == State.GoToTheBall)
        {
            if(skills.HasBall())
                skills.Shoot();
            else
            {
                skills.MoveTo(Ball.ball.transform.position);
                if(Vector3.Distance(transform.position, Ball.ball.transform.position) < 2f)
                    skills.CatchBall();   
            }
        }
    }
}
