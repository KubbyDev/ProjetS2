using Photon.Pun;
using UnityEngine;

//Ce script gere la synchronisation de la balle sur le reseau

public class BallSync : MonoBehaviour, IPunObservable
{
    void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            //TODO: Interpolate
        }
    }
    
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

                PredictBallPositionAndRotation(position, rotation, velocity, angularV, Mathf.Abs((float) (PhotonNetwork.Time - info.timestamp)));
            }
            //Si un joueur a la balle on calcule sa position en local sans prendre en compte le serveur
            //Ses physiques sont resynchronisees quand elle est lancee
        }
    }
    
    //Cette methode va utiliser les donnees accessible et le temps de trajet du message
    //pour predire la position et la rotation de la balle sur le serveur au moment ou le message est recu
    void PredictBallPositionAndRotation(Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angularV, float latency)
    {
        //Position et Vitesse

        //On trace un raycast pour savoir si on va toucher un mur
        RaycastHit hit;
        if (Physics.Raycast(position, velocity, out hit, latency * velocity.magnitude))
        {
            //Si on touche un mur on laisse les calculs se faire en local
            //et on recupere la reponse du serveur a l'update suivante
        }
        else
        {
            //Si on ne touche pas de mur, on place la balle en avant en fonction de sa vitesse et du temps ecoule
            Ball.rigidBody.velocity = velocity;
            transform.position = position + velocity * latency;
            
            //On prend en compte la gravite si la vitesse verticale est au moins de 0.1 m.s
            //Evite que la balle passe sous le sol
            if (velocity.y > 0.1f)
            {
                Ball.rigidBody.velocity += Physics.gravity * latency;
                transform.position += 0.5f * Physics.gravity * latency * latency;
            }
        }
        
        //Rotation et Vitesse angulaire
        transform.rotation = rotation * Quaternion.Euler(angularV * latency);
        Ball.rigidBody.angularVelocity = angularV;
    }
}
