using UnityEngine;

public class Skills : MonoBehaviour
{
    //Le but de cette classe et de donner a Brain.cs l'acces a tout un tas de fonctionnalites
    //de haut niveau (aller a un endroit, se demarquer, tirer, faire une passe, aller aux cages etc...)

    public float timeToMove;          //Le temps restant avant que l'IA puisse bouger
    
    private Transform cam;            //Une fausse camera qui symbolise la direction du regard de l'IA
    
    private MovementManager move;     //Reference au MovementManager de l'IA
    private BallManager ballManager;  //Reference au BallManager de l'IA
    private PlayerInfo infos;         //Reference au PlayerInfo de l'IA
    private GameObject ball;          //Reference a la balle

    void Start()
    {
        move = GetComponent<MovementManager>();
        ballManager = GetComponent<BallManager>();
        infos = GetComponent<PlayerInfo>();
        cam = transform.Find("CameraAnchor");
        UpdateBallRef();
    }

    void Update()
    {
        infos.cameraAnchor = cam;

        //Met a jour le temps restant pour pouvoir bouger
        if (timeToMove > 0)
            timeToMove -= Time.deltaTime;
    }

    //Bouger jusqu'a position
    public void MoveTo(Vector3 position)
    {
        LookAt(position);
        Vector3 moveInput = position - transform.position;
        moveInput.y = 0;
        Move(moveInput.normalized);
    }

    public void Turn(float newRotation)
    {
        transform.eulerAngles = new Vector3(0, newRotation, 0);
    }

    //Regarde le point specifie
    public void LookAt(Vector3 point)
    {
        cam.LookAt(point);
        Turn(cam.rotation.eulerAngles.y);
    }

    //Essaye d'attraper la balle
    public void CatchBall()
    {
        LookAt(ball.transform.position);
        ballManager.Catch();
    }

    private void Move(Vector3 input)
    {
        if(timeToMove <= 0)
            move.Move(input);
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
