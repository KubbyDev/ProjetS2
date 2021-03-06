﻿using Photon.Pun;
using UnityEngine;

//Le but de cette classe et de donner a Brain.cs l'acces a tout un tas de fonctionnalites
//de haut niveau (aller a un endroit, se demarquer, tirer, faire une passe, aller aux cages etc...)

public partial class Skills : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 540;
    
    public float blockInputs;         //Le temps restant avant que l'IA puisse bouger et utiliser ses spells
    
    private Transform cam;            //Une fausse camera qui symbolise la direction du regard de l'IA  
    private MovementManager move;     //Reference au MovementManager de l'IA
    private BallManager ballManager;  //Reference au BallManager de l'IA
    private PlayerInfo infos;         //Reference au PlayerInfo de l'IA
    private Hook hook;                //Reference au gestionnaire du powerup hook
    private Back back;                //Reference au gestionnaire du powerup back
    private PowerShoot powerShoot;    //Reference au gestionnaire du powerup powershoot
    private Striker striker;          //Reference au script des spells du stricker
    private Warden warden;            //Reference au script des spells du warden
    private Ninja ninja;              //Reference au script des spells du ninja
    private BasicSpell basic;         //Reference au script du basic spell

    private Vector3 targetPosition;
    private Quaternion targetRotation;
    
    void Start()
    {
        move = GetComponent<MovementManager>();
        ballManager = GetComponent<BallManager>();
        infos = GetComponent<PlayerInfo>();
        cam = transform.Find("CameraAnchor");
        hook = GetComponent<Hook>();
        back = GetComponent<Back>();
        powerShoot = GetComponent<PowerShoot>();
        striker = GetComponent<Striker>();
        warden = GetComponent<Warden>();
        ninja = GetComponent<Ninja>();
        basic = GetComponent<BasicSpell>();
    }

    void Update()
    {
        //Les IA sont controllees par le host
        if (!PhotonNetwork.IsMasterClient)
            return;
        
        if (blockInputs > 0)
        {
            //Met a jour le temps restant pour pouvoir bouger
            blockInputs -= Time.deltaTime;
        }
        else if (targetPosition != Vector3.zero)
        {
            //Se deplace vers targetPosition
            Vector3 moveInput = targetPosition - transform.position;
            moveInput.y = 0;
            move.Move(moveInput.normalized);
        }

        //Se tourne vers targetRotation
        cam.rotation = Quaternion.RotateTowards(cam.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(new Vector3(0, cam.rotation.eulerAngles.y, 0));

        infos.cameraRotation = cam.rotation;
        infos.cameraPosition = cam.position;
    }
}
