using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

//Ce script gere les physiques de la balle (L'attirance vers le joueur qui l'attrape, le tir, le freeze etc)

public class Ball : MonoBehaviour
{
    public static GameObject ball;      //Reference a la balle, visible partout
    public static Ball script;          //Reference a ce script, visible partout
    public static Rigidbody rigidBody;  //Reference au rigidbody de la balle, visible partout
    
    [SerializeField] [Range(1, 20)] private float pullStrength = 9;            //La force avec laquelle la balle est attiree au joueur qui la possede

    [HideInInspector] public static GameObject possessor { get; private set; } //Le joueur qui a la balle (null si elle est libre)
    [HideInInspector] public bool canBeCaught = true;                          //Vrai si la balle peut etre recuperee
    [HideInInspector] public GameObject shooterBlue;      //Le dernier joueur bleu a avoir lance la balle (enregistre quand possessor passe a null)
    [HideInInspector] public GameObject shooterOrange;    //Le dernier joueur orange a avoir lance la balle (enregistre quand possessor passe a null)
    [HideInInspector] public bool lastTeamIsBlue;         //True: La derniere equipe a avoir possede la balle est l'equipe bleue

    private static PhotonView pv;       //Le script qui gere la balle sur le reseau

    private float freezeTime = 0f;

    void Awake()
    {
        ball = this.gameObject;
        script = this;
        rigidBody = GetComponent<Rigidbody>();
    }
    
    void Start()
    {
        possessor = null;
        pv = GetComponent<PhotonView>();
    }

    void Update()
    {
        //Si la balle est possedee par un joueur, on l'attire devant lui
        if (possessor != null)
            Attract();
        
        if (freezeTime > 0)
            freezeTime -= Time.deltaTime; //duree du freeze - 1 par sec

        if (freezeTime <= 0)
        {
            canBeCaught = true;
            rigidBody.useGravity = true; //reset la gravite de la ball
        }
    }

    //Met a jour le possesseur de la balle chez tous les clients
    public static void UpdatePossessor(GameObject newPossessor)
    {
        //Appelle la fonction UpdatePossessor_RPC chez chaque client
        int id = newPossessor == null ? -1 : newPossessor.GetComponent<PhotonView>().ViewID;
        script.UpdatePossessor_RPC(id);
        pv.RPC("UpdatePossessor_RPC", RpcTarget.Others, id);
    }

    [PunRPC]
    //Met le hasBall de tous les joueurs a false, sauf le possesseur de balle
    private void UpdatePossessor_RPC(int viewID)
    {
        //Si le viewID est a -1 c'est qu'un joueur vient de jeter la balle
        if (viewID == -1)
            possessor = null;
        else
        {
            possessor = PhotonView.Find(viewID).gameObject;
            lastTeamIsBlue = possessor.GetComponent<PlayerInfo>().team == Team.Blue;
            if (lastTeamIsBlue)
                shooterBlue = possessor;
            else
                shooterOrange = possessor;
        }

        //On enleve la possession de balle a tous les joueurs, sauf le nouveau possesseur
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
            player.GetComponent<BallManager>().hasBall = player == possessor;
    }

    //Attire la balle au joueur qui la possede
    void Attract()
    {
        rigidBody.velocity /= 1.2f;   //Amorti la vitesse
        rigidBody.AddForce((possessor.transform.position + new Vector3(0, 0.0f, 0) + 5f * possessor.GetComponent<PlayerInfo>().cameraAnchor.forward   //Un peu devant le torse du joueur
                          - transform.position    //Pour que le vecteur aille de la balle au joueur
                           ) * Time.deltaTime * pullStrength * 1000);
    }

    public void Shoot(Vector3 force)
    {
        //Execute la fonction ShootBall_RPC sur tous les autres joueurs
        pv.RPC("ShootBall_RPC", RpcTarget.Others, rigidBody.velocity, force);

        rigidBody.AddForce(force);
        UpdatePossessor(null);
    }

    [PunRPC]
    //Cette fonction s'execute chez tous les clients
    //Elle met a jour le possesseur de balle et la vitesse de la balle
    private void ShootBall_RPC(Vector3 newVelocity, Vector3 force)
    {
        rigidBody.velocity = newVelocity;
        rigidBody.AddForce(force);
    }

    //Remet a 0 la vitesse, la rotation et la vitesse angulaire de la balle
    public static void ResetSpeed()
    {
        rigidBody.velocity = Vector3.zero;
        rigidBody.angularVelocity = Vector3.zero;
        rigidBody.rotation = Quaternion.identity;
    }

    //Replace la balle a son point de spawn
    public static void Respawn(Vector3 spawnPos)
    {
        ball.transform.position = spawnPos;
        ResetSpeed();
        UpdatePossessor(null);
        rigidBody.useGravity = true;
    }

    //Cache la balle (on la place en fait sous le terrain)
    public static void Hide()
    {
        ball.transform.position = new Vector3(0,-1000,0);
        ResetSpeed();
        UpdatePossessor(null);
        rigidBody.useGravity = false;
    }
    
    //Freeze la balle
    public void FreezeBall()
    {
        canBeCaught = false; //ne peut etre attrape
        freezeTime = 6f; //duree du freeze
        rigidBody.useGravity = false; 
        ResetSpeed(); //freeze la ball
    }
}
