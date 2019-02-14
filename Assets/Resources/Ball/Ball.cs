using Photon.Pun;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] [Range(1, 20)] private float pullStrength = 9;    //La force avec laquelle la balle est attiree au joueur qui la possede

    public GameObject possessor { get; private set; }    //Le joueur qui a la balle (null si elle est libre)
    public bool canBeCaught = true;                      //Vrai si la balle peut etre recuperee
    private Rigidbody rb;                                //Le component qui gere les physiques de la balle
    private PhotonView pv;                               //Le script qui gere la balle sur le reseau

    void Start()
    {
        possessor = null;
        rb = GetComponent<Rigidbody>();
        pv = GetComponent<PhotonView>();
    }

    void Update()
    {
        if (possessor != null)
            Attract();
    }

    public void UpdatePossessor(GameObject newPossessor)
    {
        //Appelle la fonction UpdatePossessor_RPC chez chaque client
        pv.RPC("UpdatePossessor_RPC", RpcTarget.All, newPossessor == null ? -1 : newPossessor.GetComponent<PhotonView>().ViewID);
    }

    [PunRPC]
    //Met le hasBall de tous les joueurs a false, sauf le possesseur de balle
    private void UpdatePossessor_RPC(int viewID)
    {
        possessor = viewID == -1 ? null : PhotonView.Find(viewID).gameObject;

        //On enleve la possession de balle a tous les joueurs, sauf le nouveau possesseur
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
            if (player == possessor)
                player.GetComponent<BallManager>().hasBall = true;
            else
                player.GetComponent<BallManager>().hasBall = false;
    }

    //Attire la balle au joueur qui la possede
    void Attract()
    {
        rb.velocity /= 1.2f;                                                                                     //Amorti la vitesse
        rb.AddForce((possessor.transform.position + new Vector3(0, 0.0f, 0) + 5f * possessor.transform.forward   //Un peu devant le torse du joueur
                        - transform.position                                                                     //Pour que le vecteur aille de la balle au joueur
                        ) * Time.deltaTime * pullStrength * 1000);
    }

    public void Shoot(Vector3 force)
    {
        //Execute la fonction ShootBall_RPC sur tous les autres joueurs
        pv.RPC("ShootBall_RPC", RpcTarget.Others, rb.velocity, force);

        rb.AddForce(force);
        UpdatePossessor(null);
    }

    [PunRPC]
    //Cette fonction s'execute chez tous les clients
    //Elle met a jour le possesseur de balle et la vitesse de la balle
    private void ShootBall_RPC(Vector3 newVelocity, Vector3 force)
    {
        Debug.Log("Set velocity");

        rb.velocity = newVelocity;
        rb.AddForce(force);
    }

    public void FreezeBall()
    {
        canBeCaught = false;
    }

    public void DeFreezeBall()
    {
        canBeCaught = true;
    }
}
