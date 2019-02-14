using Photon.Pun;
using UnityEngine;

public class BallSync : MonoBehaviour, IPunObservable
{
    private Rigidbody ballRB;                   //Le rigid body de la balle
    private Ball ball;                          //Une reference au script Ball de la balle

    void Start()
    {
        ballRB = GetComponent<Rigidbody>();
        ball = GetComponent<Ball>();
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
            if (ball.possessor == null)
            //Si la balle est libre on va juste calculer sa position entre les updates du reseau en simulant ses physiques
            {
                transform.position     = (Vector3)    stream.ReceiveNext();
                transform.rotation     = (Quaternion) stream.ReceiveNext();
                ballRB.velocity        = (Vector3)    stream.ReceiveNext();
                ballRB.angularVelocity = (Vector3)    stream.ReceiveNext();
            }
            //Si un joueur a la balle on calcule sa position en local sans prendre en compte le serveur
            //Ses physiques sont resynchronisees quand elle est lancee
        }
    }
}
