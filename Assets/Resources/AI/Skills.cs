using System.Collections;
using System.Collections.Generic;
using Boo.Lang.Environments;
using UnityEngine;

public class Skills : MonoBehaviour
{
    //Le but de cette classe et de donner a Brain.cs l'acces a tout un tas de fonctionnalites
    //de haut niveau (aller a un endroit, se demarquer, tirer, faire une passe, aller aux cages etc...)

    public enum State
    {
        GoToTheBall = 1
    }

    public State currentState;

    private Transform camera;           //Une fausse camera qui symbolise la direction du regard de l'IA
    
    private MovementManager move;
    private BallManager ballManager;
    private PlayerInfo infos;
    private GameObject ball;

    void Start()
    {
        move = GetComponent<MovementManager>();
        ballManager = GetComponent<BallManager>();
        infos = GetComponent<PlayerInfo>();
        camera = transform.Find("CameraAnchor");
        UpdateBallRef();
    }

    void Update()
    {
        infos.cameraAnchor = camera;
        
        if (currentState == State.GoToTheBall && !infos.hasBall)
        {
            MoveTo(ball.transform.position);
            ballManager.Catch();
        }
    }

    public void MoveTo(Vector3 position)
    {
        LookAt(position);
        Vector3 moveInput = position - transform.position;
        moveInput.y = 0;
        move.Move(moveInput.normalized);
    }

    public void Turn(float newRotation)
    {
        transform.eulerAngles = new Vector3(0, newRotation, 0);
    }

    public void LookAt(Vector3 point)
    {
        camera.LookAt(point);
        Turn(camera.rotation.eulerAngles.y);
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
}
