using Photon.Pun;
using UnityEngine;

public class PlayerSync : MonoBehaviour, IPunObservable
{
    [SerializeField] private float rotationSpeed = 500;
    
    private Vector3 movementInput;
    private Quaternion targetRotation;
    
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

        move.Move(movementInput);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime*rotationSpeed);
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) //Donnees envoyees
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(infos.velocity);
            stream.SendNext(infos.lastMovementInput);
        }
        else                  //Donnees recues
        {
            transform.position = (Vector3)    stream.ReceiveNext();
            targetRotation     = (Quaternion) stream.ReceiveNext();
            move.velocity      = (Vector3)    stream.ReceiveNext();
            movementInput      = (Vector3)    stream.ReceiveNext();
        }
    }
}
