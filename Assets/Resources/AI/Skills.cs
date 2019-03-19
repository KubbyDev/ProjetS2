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

    void Start()
    {
        move = GetComponent<MovementManager>();
        ballManager = GetComponent<BallManager>();
        infos = GetComponent<PlayerInfo>();
        cam = transform.Find("CameraAnchor");
    }

    void Update()
    {
        infos.cameraAnchor = cam;

        //Met a jour le temps restant pour pouvoir bouger
        if (timeToMove > 0)
            timeToMove -= Time.deltaTime;
    }

    // Competences -----------------------------------------------------------------------------------------------------
    
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

    // Fonctions basiques ----------------------------------------------------------------------------------------------
    
    public void Turn(float newRotation)
    {
        if(timeToMove <= 0)
            transform.eulerAngles = new Vector3(0, newRotation, 0);
    }
    
    //Regarde le point specifie
    public void LookAt(Vector3 point)
    {
        cam.LookAt(point);
        Turn(cam.rotation.eulerAngles.y);
    }
    
    // Fonctions annexes -----------------------------------------------------------------------------------------------
    
    private void Move(Vector3 input)
    {
        if(timeToMove <= 0)
            move.Move(input);
    }
}
