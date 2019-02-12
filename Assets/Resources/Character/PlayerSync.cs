using Photon.Pun;
using UnityEngine;

public class PlayerSync : MonoBehaviour//, IPunObservable
{

    //Marche plutot bien mais il y a des lags


    /*
    private MovementManager move;
    private Vector3 movementInput;
    private PhotonView pv;

    void Start()
    {
        move = GetComponent<MovementManager>();
        pv = GetComponent<PhotonView>();
    }

    void Update()
    {
        if(! pv.IsMine)
            move.Move(movementInput);
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) //Donnees envoyees
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(move.velocity);
            stream.SendNext(move.getLastMovementInput());
        }
        else                  //Donnees recues
        {
            transform.position = (Vector3)    stream.ReceiveNext();
            transform.rotation = (Quaternion) stream.ReceiveNext();
            move.velocity      = (Vector3)    stream.ReceiveNext();
            movementInput      = (Vector3)    stream.ReceiveNext();
        }
    }
    */
}
