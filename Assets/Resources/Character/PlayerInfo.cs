using Photon.Pun;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public static GameObject localPlayer;  //Une reference au GameObject de ce client (initialise dans PlayerSetup)

    //Ces variables sont stockees ici, si vous les modifiez,
    //ca aura une influence sur le jeu
    public int goalsScored;                //Le nombre de buts que le joueur a marque
    public Team team = Team.None;          //La team du joueur
    public Hero hero = Hero.Stricker;      //La classe jouee par ce joueur
    public bool isPlayer;                  //False: C'est une IA
    public int ping;                       //Le ping de ce joueur
    
    //Ces variables sont simplement copiees ici
    //Les modifier n'aura aucun effet
    public Quaternion rotation;            //L'orientation de la camera
    public Transform cameraAnchor;         //L'ancre de la camera
    public Vector3 velocity;               //La vitesse du joueur
    public bool isGrounded;                //Si le joueur est au sol
    public bool hasBall;                   //Si le joueur a la balle

    private float timeToPingUpdate;
    private PhotonView pv;

    void Awake()
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

    public void SetTeam(Team t)
    {
        GetComponent<MeshRenderer>().material.color = t == Team.Blue ? new Color(0, 82, 204, 1) : new Color(230, 92, 0, 1);
        team = t;
    }
    
    public void SetHero(Hero h)
    {
        hero = h;
    }

    //Synchronise les infos de ce joueur chez tous les clients
    public void UpdateInfos()
    {
        pv.RPC("UpdateInfo_RPC", RpcTarget.Others, (int) team, (int) hero);
    }

    [PunRPC]
    public void UpdateInfo_RPC(int pTeam, int pHero)
    {       
        SetTeam((Team) pTeam);
        SetHero((Hero) pHero);
    }
}
