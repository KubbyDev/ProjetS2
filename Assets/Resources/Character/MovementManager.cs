﻿using Photon.Pun;
using UnityEngine;

//Cette classe recoit des inputs de PlayerInputManager pour les actions qui font bouger le joueur

public class MovementManager : MonoBehaviour
{
    [Header("Jumps")]
    [SerializeField] [Range(0, 50)] private float jumpStrength = 10;      //La force des sauts
    [SerializeField] [Range(0, 10)] private int maxJumps = 3;             //Le nombre max de sauts sans toucher le sol
    [SerializeField] [Range(0, 50)] private float dashesStrength = 20f;   //La force des dashes (forces horizontales quand le joueur appuie sur espace + ZQSD)
    [Space] [Header("Movements")]
    [SerializeField] [Range(0, 2000)] private float movementSpeed = 500;  //La vitesse des deplacements au sol
    [SerializeField] [Range(0, 2)] private float inAirControl = 1.2f;     //La force des inputs en l'air (en l'air: inputs *= inAirControl/vitesse^2)
    [SerializeField] [Range(1, 100)] private float maxSpeed = 20f;        //La vitesse maximale de deplacement du joueur

    [HideInInspector] public Vector3 velocity = Vector3.zero;             //La vitesse actuelle du joueur

    private CharacterController cc;       //Le script qui gere les deplacements du joueur (dans Unity)
    private Transform camAnchor;          //Le pivot de la camera
    private int usableJumps;              //Le nombre de sauts restants (Reset quand le sol est touche)
    private Vector3 movementInput;        //Le dernier input ZQSD du joueur (sert pour la synchronisation)
    private PhotonView pv;                //Le script qui gere ce joueur sur le reseau

    void Start()
	{
        camAnchor = transform.Find("CameraAnchor");
        cc = GetComponent<CharacterController>();
        pv = GetComponent<PhotonView>();
    }

    void FixedUpdate()
    {
        //Vitesse max
        if(velocity.sqrMagnitude > maxSpeed*maxSpeed)
            velocity -= velocity * Time.fixedDeltaTime; //Revient a la vitesse max autorisee

        //Gravity
        //Pour supprimer l'impression de faible gravite on l'augmente quand le joueur tombe
        velocity += Physics.gravity * Time.fixedDeltaTime * (velocity.y < 0 ? 2f : 1.0f);

        //Fait bouger le joueur
        cc.Move(velocity * Time.fixedDeltaTime);

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
    public void Move(Vector3 input) 
	{
        movementInput = input;   //On enregistre l'input du joueur pour la synchronisation

        if (input.sqrMagnitude > 0)
        {
            input = input.z * transform.forward + input.x * transform.right;
            input.y = 0;

            velocity += input.normalized * Time.deltaTime * movementSpeed             //Le vecteur d'inputs en temps normal
                * (cc.isGrounded ? 1 : inAirControl / (velocity.sqrMagnitude + 2));   //Quand le joueur est en l'air on multiplie pa
        }                                                                             //inAirControl/velocity.sqrMagnitude, +2 pour eviter la division par 0 et les a coups
    }

    public Vector3 getLastMovementInput()
    {
        return movementInput;
    }

    //Appellee par InputManager
    public void Jump(Vector3 movementInput)  //On prend en parametre les inputs ZQSD pour savoir si on doit appliquer une force horizontale
    {
        if (usableJumps > 0)
        {
            usableJumps--;

            //On reduit la vitesse verticale si le joueur est en chute
            if (velocity.y < 0)
                velocity.y /= 5;

            if (movementInput.sqrMagnitude > 0 && !cc.isGrounded)
            //Dash
                AddForce((movementInput.z * camAnchor.transform.forward + movementInput.x * camAnchor.transform.right).normalized * dashesStrength);
            else
            //Saut classique
                AddForce(new Vector3(0, jumpStrength, 0));
        }
    }

    //Applique une force sur le joueur
    public void AddForce(Vector3 force)  
	{
        pv.RPC("ApplyForce_RPC", RpcTarget.All, force);
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
}
