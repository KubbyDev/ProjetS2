using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

//Cette classe gere les mouvements du joueur

public class MovementManager : MonoBehaviour
{
    [Header("Jumps")]
    [SerializeField] [Range(0, 50)] private float jumpStrength = 10;      //La force des sauts
    [SerializeField] [Range(0, 10)] private int maxJumps = 3;             //Le nombre max de sauts sans toucher le sol
    [Space] [Header("Movements")]
    [SerializeField] [Range(0, 2000)] private float baseMovementSpeed = 500; //La vitesse de deplacements de base (reste constante)
    [SerializeField] [Range(0, 0.5f)] private float inAirControl = 0.03f;    //La force des inputs en l'air (en l'air: inputs *= inAirControl/vitesse^2)
    [SerializeField] [Range(0, 1f)] private float withBallSpeed = 0.80f;     //Le multiplicateur de vitesse quand le joueur a la balle
    [SerializeField] [Range(1, 100)] private float maxSpeed = 20f;           //La vitesse maximale de deplacement du joueur

    public Vector3 velocity = Vector3.zero;   //La vitesse actuelle du joueur
    public float movementSpeed;               //La vitesse de deplacement actuelle (peut etre modifiee)
    
    private CharacterController cc;           //Le script qui gere les deplacements du joueur (dans Unity)
    private int usableJumps;                  //Le nombre de sauts restants (Reset quand le sol est touche)
    private int usableDashes;                 //Le nombre de dashes restants (Reset quand le sol est touche)
    private Vector3 movementInput;            //Le dernier input ZQSD du joueur (sert pour la synchronisation)
    private PhotonView pv;                    //Le script qui gere ce joueur sur le reseau
    private PlayerInfo infos;                 //Le script qui contient les infos sur le joueur
    private List<IEnumerator> speedCoroutines;      //References aux coroutines MultiplySpeed lancees (permet de les stopper a l'engagement)

    void Start()
    {
        movementSpeed = baseMovementSpeed;
        cc = GetComponent<CharacterController>();
        pv = GetComponent<PhotonView>();
	    infos = GetComponent<PlayerInfo>();
        speedCoroutines = new List<IEnumerator>();
    }

    void Update()
    {
        //Vitesse max
        if(velocity.sqrMagnitude > maxSpeed*maxSpeed)
            velocity -= velocity * Time.deltaTime; //Revient a la vitesse max autorisee

        //Gravity
        //Pour supprimer l'impression de faible gravite on l'augmente quand le joueur tombe
        velocity += Physics.gravity * Time.deltaTime * (velocity.y < 0 ? 2f : 1.0f);

        //Fait bouger le joueur
        cc.Move(velocity * Time.deltaTime);

        infos.velocity = cc.velocity;
        infos.isGrounded = cc.isGrounded;
        
        if (cc.isGrounded) //Quand le joueur est au sol
        {
            velocity = Vector3.zero;
            usableJumps = maxJumps;
        }
        else               //Quand le joueur est en l'air
        {
            //cc.velocity est la vitesse reele du CharacterController (elle tient compte des collisions)
            velocity = cc.velocity;
        }
    }

    //Appellee par InputManager
    //Prend un vecteur normalise avec y=0
    public void Move(Vector3 input)
    {
        infos.lastMovementInput = input;
        
        if (input.sqrMagnitude > 0)
            velocity += input * Time.deltaTime * movementSpeed  //Le vecteur d'inputs en temps normal
                * (cc.isGrounded ? 1 : inAirControl)            //Quand le joueur est en l'air on multiplie par inAirControl
                * (infos.hasBall ? withBallSpeed : 1);          //Quand le joueur a la balle on reduit sa vitesse
    }

    //Appellee par InputManager
    public void Jump() 
    {
        //Si le joueur ne peut pas sauter on n'execute pas le code de saut
        if (usableJumps <= 0) 
            return;
        
        usableJumps--;

        //On reduit la vitesse verticale si le joueur est en chute
        if (velocity.y < 0)
            velocity.y /= 5;
        
        //On ajoute la force: Une force vers le haut, un peu penchee dans la direction des inputs
        AddForce(new Vector3(0,jumpStrength, 0));
    }

    //Applique une force sur le joueur (sur son client)
    public void AddForce(Vector3 force)  
	{
        pv.RPC("ApplyForce_RPC", pv.Owner, force);
	}

    [PunRPC]
    private void ApplyForce_RPC(Vector3 force)
    {
        velocity += force;
    }

    //Multiplie la vitesse de deplacement par multiplier
    public void MultiplySpeed(float multiplier)
    {
        movementSpeed *= multiplier;
    }

    //Multiplie la vitesse de deplacement par multiplier puis la remet a sa valeur initiale apres duration secondes
    public void MultiplySpeed(float multiplier, float duration)
    {
        //Envoie la commande a tous les clients
        pv.RPC("MultiplySpeed_RPC", RpcTarget.All, multiplier, duration, PhotonNetwork.Time);
    }
    
    [PunRPC]
    public void MultiplySpeed_RPC(float multiplier, float duration, double sendMoment)
    {
        IEnumerator speedCoroutine = MultiplySpeedCoroutine(multiplier, duration - Tools.GetLatency(sendMoment));
        speedCoroutines.Add(speedCoroutine);
        StartCoroutine(speedCoroutine);
    }

    IEnumerator MultiplySpeedCoroutine(float multiplier, float duration)
    {
        MultiplySpeed(multiplier);
        yield return new WaitForSeconds(duration);
        MultiplySpeed(1 / multiplier);
    }

    public void ResetSpeed()
    {
        velocity = Vector3.zero;
        movementSpeed = baseMovementSpeed;
        foreach (IEnumerator speedCoroutine in speedCoroutines)
            StopCoroutine(speedCoroutine);
        speedCoroutines = new List<IEnumerator>();
    }
}
