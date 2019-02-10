using Photon.Pun;
using UnityEngine;

public class BallSync : MonoBehaviour //, IPunObservable
{


    //Marche pas bien quand un joueur tiens la balle


    /*
    private Rigidbody ballRB;

    void Start()
    {
        ballRB = GetComponent<Rigidbody>();
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) //Donnees envoyees
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(ballRB.velocity);
            stream.SendNext(ballRB.angularVelocity);
        }
        else                  //Donnees recues
        {
            transform.position     = (Vector3)    stream.ReceiveNext();
            transform.rotation     = (Quaternion) stream.ReceiveNext();
            ballRB.velocity        = (Vector3)    stream.ReceiveNext();
            ballRB.angularVelocity = (Vector3)    stream.ReceiveNext();
        }
    }
    */
}
