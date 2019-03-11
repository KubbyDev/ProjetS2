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
            if (Ball.possessor == null)
            //Si la balle est libre on va juste calculer sa position entre les updates du reseau en simulant ses physiques
            {
                Vector3 position    = (Vector3)    stream.ReceiveNext();
                Quaternion rotation = (Quaternion) stream.ReceiveNext();
                Vector3 velocity    = (Vector3)    stream.ReceiveNext();
                Vector3 angularV    = (Vector3)    stream.ReceiveNext();

                PredictBallPositionAndRotation(position, rotation, velocity, angularV, (float) info.timestamp);
            }
            //Si un joueur a la balle on calcule sa position en local sans prendre en compte le serveur
            //Ses physiques sont resynchronisees quand elle est lancee
        }
    }
    
    //Cette methode va utiliser les donnees accessible et le temps de trajet du message
    //pour predire la position et la rotation de la balle sur le serveur au moment ou le message est recu
    void PredictBallPositionAndRotation(Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angularV, float timestamp)
    {
        //Position
        
        //On trace un raycast pour savoir si on va toucher un mur
        RaycastHit hit;
        if (Physics.Raycast(transform.position, velocity, out hit, timestamp * velocity.magnitude))
        {
            //Si on touche un mur on laisse les calculs se faire en local
            //et on recupere la reponse du serveur a l'update suivante
            ;
        }
        else
        {
            //Si on ne touche pas de mur, on place la balle en avant en fonction de sa vitesse et du temps ecoule
            transform.position = position + velocity * timestamp;
        }

        //Rotation
        
        transform.rotation = rotation * Quaternion.Euler(angularV * timestamp);

        //Vitesse
        
        Ball.rigidBody.velocity = velocity;
        Ball.rigidBody.angularVelocity = angularV;
    }
}
