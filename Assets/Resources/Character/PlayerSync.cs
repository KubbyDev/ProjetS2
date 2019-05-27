using Photon.Pun;
using UnityEngine;

//Cette classe gere la synchronisation des joueurs sur le reseau

public class PlayerSync : MonoBehaviour, IPunObservable
{
    public const float rotationSpeed = 500;       //La vitesse de l'interpolation de l'orientation
    public const float interpolationSpeed = 0.5f; //La vitesse de l'interpolation de la position
    
    private Vector3 movementInput;     //Le dernier input ZQSD du joueur concerne
    private Quaternion targetRotation; //La derniere orientation recue
    private Vector3 targetPosition;    //La derniere position recue
    
    private PhotonView pv; 
    private PlayerInfo infos;
    private MovementManager move;

    void Awake()
    {
        move = GetComponent<MovementManager>();
        pv = GetComponent<PhotonView>();
        infos = GetComponent<PlayerInfo>();
    }

    void Update()
    {
        //Si c'est le joueur local on ne fait rien
        if (pv.IsMine)
            return;

        //Rapproche le joueur de la derniere position recue
        InterpolateMovement();
        
        //Fait bouger le joueur avec le dernier input de mouvement recu
        move.Move(movementInput);
        
        //On fait en sorte que l'interpolation se fasse vers un point qui bouge dans la meme direction que le joueur
        targetPosition += infos.velocity * Time.deltaTime;
        
        //Fait tourner le joueur de facon fluide
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime*rotationSpeed);
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) //Donnees envoyees
        {
            stream.SendNext(transform.position);
            stream.SendNext(infos.cameraRotation);
            stream.SendNext(infos.velocity);
            stream.SendNext(infos.lastMovementInput);
        }
        else                  //Donnees recues
        {
            targetPosition = (Vector3)    stream.ReceiveNext();
            targetRotation = (Quaternion) stream.ReceiveNext();
            move.velocity  = (Vector3)    stream.ReceiveNext();
            movementInput  = (Vector3)    stream.ReceiveNext();

            //On met l'orientation reele pour la camera, et l'orientation horizontale seulement pour le joueur (l'avatar ne se penche pas)
            infos.cameraRotation = targetRotation;
            targetRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
        }
    }

    void InterpolateMovement()
    {
        transform.position = (1-interpolationSpeed) * transform.position + interpolationSpeed * targetPosition;
    }
}
