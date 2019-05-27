using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

//Ce script gere les physiques de la balle (L'attirance vers le joueur qui l'attrape, le tir, le freeze etc)

public class Ball : MonoBehaviour
{
    public const float pullStrength = 140;            //La force avec laquelle la balle est attiree au joueur qui la possede
    
    public static GameObject ball;       //Reference a la balle, visible partout
    public static Ball script;           //Reference a ce script, visible partout
    public static Rigidbody rigidBody;   //Reference au rigidbody de la balle, visible partout
    public static PhotonView photonView; //Le script qui gere la balle sur le reseau
    public static GameObject possessor;  //Le joueur qui a la balle (null si elle est libre)
    public bool canBeCaught = true;      //Vrai si la balle peut etre recuperee
    public GameObject shooterBlue;       //Le dernier joueur bleu a avoir lance la balle (enregistre quand possessor passe a null)
    public GameObject shooterOrange;     //Le dernier joueur orange a avoir lance la balle (enregistre quand possessor passe a null)
    public bool lastTeamIsBlue;          //True: La derniere equipe a avoir possede la balle est l'equipe bleue

    private BallParticles ballParticles;
    private float freeze1Time = 0f;
    private float freeze2Time = 0f;

    void Awake()
    {
        ball = this.gameObject;
        script = this;
        rigidBody = GetComponent<Rigidbody>();
        ballParticles = GetComponent<BallParticles>();
    }
    
    void Start()
    {
        possessor = null;
        photonView = GetComponent<PhotonView>();
    }

    void Update()
    {
        //Si la balle est possedee par un joueur, on l'attire devant lui
        if (possessor != null)
            Attract();
        
        if (freeze1Time > 0)
            freeze1Time -= Time.deltaTime; //duree du freeze1 - 1 par sec
        if (freeze2Time > 0)
            freeze2Time -= Time.deltaTime; //duree du freeze2 - 1 par sec

        //Freeze1 = Balle bloquee en l'air, non attrapable
        if (freeze1Time <= 0)
            canBeCaught = true;

        //Freeze2 = Balle bloquee en l'air mais attrapable
        if (freeze2Time <= 0)
        {
            rigidBody.useGravity = true; //reset la gravite de la ball   
            ballParticles.OnFreezeStop();
        }
    }

    //Met a jour le possesseur de la balle chez tous les clients
    public static void UpdatePossessor(GameObject newPossessor)
    {
        //Appelle la fonction UpdatePossessor_RPC chez chaque client
        int id = newPossessor == null ? -1 : newPossessor.GetComponent<PhotonView>().ViewID;
        script.UpdatePossessor_RPC(id);
        photonView.RPC("UpdatePossessor_RPC", RpcTarget.Others, id);
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
        
        //On reset le temps de freeze au cas ou il serait encore en cours
        if (freeze2Time > 0)
        {
            freeze1Time = 0;
            freeze2Time = 0;
        
            ballParticles.OnFreezeStop();
        }
        
        //Change la couleur de la balle
        ballParticles.UpdateColor(lastTeamIsBlue);
    }

    //Attire la balle au joueur qui la possede
    void Attract()
    {
        rigidBody.velocity /= 1.2f;   //Amorti la vitesse
        rigidBody.AddForce((possessor.transform.position + new Vector3(0, 0.0f, 0) + 5f * (possessor.GetComponent<PlayerInfo>().cameraRotation * Vector3.forward)   //Un peu devant le torse du joueur
                          - transform.position    //Pour que le vecteur aille de la balle au joueur
                           ) * pullStrength);
    }

    public void Shoot(Vector3 force, bool powershoot)
    {
        //Execute la fonction ShootBall_RPC sur tous les autres joueurs
        photonView.RPC("ShootBall_RPC", RpcTarget.Others, rigidBody.velocity, force, powershoot);

        rigidBody.AddForce(force);
        UpdatePossessor(null);
        
        if(powershoot)
            transform.Find("FlamesParticles").GetComponent<ParticleSystem>().Play();
    }

    [PunRPC]
    //Cette fonction s'execute chez tous les clients
    //Elle met a jour le possesseur de balle et la vitesse de la balle
    private void ShootBall_RPC(Vector3 newVelocity, Vector3 force, bool powershoot)
    {
        rigidBody.velocity = newVelocity;
        rigidBody.AddForce(force);
        
        if(powershoot)
            transform.Find("FlamesParticles").GetComponent<ParticleSystem>().Play();
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
        script.ballParticles.ResetColor();
        script.ballParticles.OnFreezeStop();
    }

    //Cache la balle (on la place en fait sous le terrain)
    public static void Hide()
    {
        ball.transform.position = new Vector3(0,-1000,0);
        ResetSpeed();
        UpdatePossessor(null);
        rigidBody.useGravity = false;
    }
    
    [PunRPC]
    public void Freeze(Vector3 position, float freeze1, float freeze2, double sendMoment)
    {
        float latency = Tools.GetLatency(sendMoment);
        
        canBeCaught = false; //ne peut etre attrape
        rigidBody.useGravity = false;
        ResetSpeed();

        transform.position = position;

        //Freeze1 = Balle bloquee en l'air, non attrapable
        //Freeze2 = Balle bloquee en l'air mais attrapable
        freeze1Time = freeze1 - latency;
        freeze2Time = freeze2 - latency;
        
        ballParticles.OnFreeze(freeze1,freeze2);
    }
}
