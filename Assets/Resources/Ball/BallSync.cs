using Photon.Pun;
using UnityEngine;

public class BallSync : MonoBehaviour, IPunObservable
{
    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) //Donnees envoyees
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(Ball.rigidBody.velocity);
            stream.SendNext(Ball.rigidBody.angularVelocity);
        }
        else                  //Donnees recues
        {
            if (Ball.script.possessor == null)
            //Si la balle est libre on va juste calculer sa position entre les updates du reseau en simulant ses physiques
            {
                transform.position     = (Vector3)    stream.ReceiveNext();
                transform.rotation     = (Quaternion) stream.ReceiveNext();
                Ball.rigidBody.velocity        = (Vector3)    stream.ReceiveNext();
                Ball.rigidBody.angularVelocity = (Vector3)    stream.ReceiveNext();
            }
            //Si un joueur a la balle on calcule sa position en local sans prendre en compte le serveur
            //Ses physiques sont resynchronisees quand elle est lancee
        }
    }
}
