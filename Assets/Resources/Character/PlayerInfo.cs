using Photon.Pun;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public static GameObject localPlayer;  //Une reference au GameObject de ce client (initialise dans PlayerSetup)

    public int goalsScored;                //Le nombre de buts que le joueur a marque
    public Team team;                      //La team du joueur
    public Quaternion rotation;            //L'orientation de la camera
    public Transform cameraAnchor;         //L'ancre de la camera
    public Vector3 velocity;               //La vitesse du joueur
    public bool isGrounded;                //Si le joueur est au sol
    public bool hasBall;                   //Si le joueur a la balle
    public bool isPlayer;                  //False: C'est une IA
    public int ping;                       //Le ping de ce joueur

    private float timeToPingUpdate;
    private PhotonView pv;

    void Start()
    {
        pv = GetComponent<PhotonView>();
    }

    void Update()
    {
        //Si ce n'est pas le joueur local on ne fait rien
        if ( ! (isPlayer && pv.IsMine))
            return;
        
        //Sinon on update le ping toutes les 2 secondes
        if (timeToPingUpdate > 0)
            timeToPingUpdate -= Time.deltaTime;
        else
        {
            timeToPingUpdate = 2;
            pv.RPC("UpdatePing", RpcTarget.All, PhotonNetwork.GetPing());
        }
    }

    [PunRPC]
    private void UpdatePing(int newPing)
    {
        ping = newPing;
    }
}
