using Photon.Pun;
using UnityEngine;

public class BallSync : MonoBehaviour, IPunObservable
{
    private Rigidbody ballRB;
    private GameObject ballPossessor = null;

    void Start()
    {
        ballRB = GetComponent<Rigidbody>();
    }

    void Update()
    {
        //Pas du tout opti mais en attendant ça marche
        ballPossessor = null;
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
            if (player.GetComponent<BallManager>().hasBall)
                ballPossessor = player;

        ballRB.useGravity = true;
        //Si un joueur a la balle on la place devant lui sauf si c'est le joueur local (dans ce cas c'est gere dans BallManager)
        if (ballPossessor != null && !ballPossessor.GetComponent<PhotonView>().IsMine)
        {
            ballRB.useGravity = false;
            ballRB.velocity = Vector3.zero;
            transform.position = (ballPossessor.transform.position + new Vector3(0, 0.0f, 0) + 3f * ballPossessor.transform.forward);
        }
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
            if (ballPossessor == null)
            //Si la balle est libre on va juste calculer sa position entre les updates du reseau en simulant ses physiques
            {
                transform.position     = (Vector3)    stream.ReceiveNext();
                transform.rotation     = (Quaternion) stream.ReceiveNext();
                ballRB.velocity        = (Vector3)    stream.ReceiveNext();
                ballRB.angularVelocity = (Vector3)    stream.ReceiveNext();
            }
            //Si un joueur a la balle on la place devant lui (Dans Update)
        }
    }
}
