using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brain : MonoBehaviour
{ 
    private Skills skills;            //Le script qui effectue les mouvement que ce script ordonne
    private GameObject ball;
    private PlayerInfo infos;

    // Setup des infos importantes -------------------------------------------------------------------------------------
    
    void Start()
    {
        skills = GetComponent<Skills>();
        infos = GetComponent<PlayerInfo>();
        UpdateBallRef();
    }
    
    //Met a jour la reference a la balle
    //Cette methode peut etre appellee sans argument: elle cherchera la balle elle meme, ou avec la balle en argument
    public void UpdateBallRef(GameObject newRef = null)
    {
        if (newRef == null)
            ball = GameObject.FindGameObjectWithTag("Ball");
        else
            ball = newRef;
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
        
        if (currentState == State.GoToTheBall && !infos.hasBall)
        {
            skills.MoveTo(ball.transform.position);
            skills.CatchBall();
        }
    }
}
