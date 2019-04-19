using Photon.Pun;
using System.Collections;
using UnityEngine;

public class Ninja : MonoBehaviour
{
    [SerializeField] private float Explode_Spell_Duration = 1f;   //Duree du speed
    [SerializeField] private float Explode_Cooldown = 20f;        //Cooldown du spell
    [SerializeField] private float Explode_Speed_Boost = 2f;      //Force du speed
    [SerializeField] private float Explosion_Radius = 2.0f;       //Rayon dans lequel les joueurs subissent l'explosion
    [SerializeField] private float Explosion_Power = 2f;          //Puissance de l'explosion
 
    [SerializeField] private float Smoke_Spell_Duration = 5f;     //Duree d'emission de la smoke            
    [SerializeField] private float Smoke_Cooldown = 15f;          //Cooldown du spell smoke
    [SerializeField] private float Smoke_Delay = 2f;              //Duree de l'existence de SmokeBomb avant d'exploser
    [SerializeField] private GameObject SmokeBomb;                //Prefab de la bombe pour le smoke
    [SerializeField] private GameObject SmokeExplosion;           //Prefab du ParticleSystem

    private bool Can_Smoke = true;               //Bool pour savoir si cooldown est finie ou pas
    private bool Explode_Off_Cooldown = true;    //true = cooldown de Explode fini
    
    private MovementManager move;                //Reference au script qui gere les mouvements du joueur
    private PhotonView pv;                       //Le script qui gere cet objet sur le reseau
    private PlayerInfo info;                     //Reference au script qui gere la camera du joueur

    void Awake()
    {
        move = GetComponent<MovementManager>();
        info = GetComponent<PlayerInfo>();
        pv = GetComponent<PhotonView>();
    }

    public void Explode_Spell()
    {
        if(Explode_Off_Cooldown)
            StartCoroutine(ExplodeCoroutine());
    }

    IEnumerator ExplodeCoroutine()
    { 
        move.MultiplySpeed(Explode_Speed_Boost);                             // Augmente la vitesse
        Explode_Off_Cooldown = false;
        yield return new WaitForSeconds(Explode_Spell_Duration);             // Lance le cooldown
        move.MultiplySpeed(1 / Explode_Speed_Boost);                         // Remet la vitesse normale
        Explosion();
        yield return new WaitForSeconds(Explode_Cooldown);
        Explode_Off_Cooldown = true;                                         // Le spell redevient utilisable une fois le cooldown ecoule
    }

    public void Explosion()
    {
        //Renvoie la liste des joueurs presents sur le terrain
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        //Pour chaque joueur
        foreach(GameObject player in players)
        {
            float distance = (player.transform.position - this.transform.position).magnitude;
            
            //Si le joueur est trop proche, et que ce n'est pas celui qui a lance le spell
            if (player != this.gameObject && distance <= Explosion_Radius)
            {
                //On le propulse
                Vector3 Blast = (new Vector3(player.transform.position.x - transform.position.x, 
                                    0f, 
                                    player.transform.position.z - transform.position.z
                                    ).normalized + new Vector3(0,1.0f,0))
                                * (1 - distance/Explosion_Radius);
                
                player.GetComponent<MovementManager>().AddForce(Blast * Explosion_Power);
            }
        }
    }

    public void Smoke()
    {
        if (Can_Smoke)
            StartCoroutine(SmokeCouroutine());
    }

    IEnumerator SmokeCouroutine()
    {
        Can_Smoke = false;
        
        Vector3 position = transform.position + new Vector3(0, 1.5f, 0) + transform.forward * 1.0f;
        Vector3 direction = info.cameraAnchor.forward;
        //Cree SmokeBomb
        GameObject bomb = Instantiate(SmokeBomb, 
            position, 
            Quaternion.identity);
        //Envoie la requete de spawn de la bombe
        pv.RPC("SpawnSmoke", RpcTarget.Others, position, direction, (float) PhotonNetwork.Time);   
        //Applique une force
        bomb.GetComponent<Rigidbody>().AddForce(direction * 1000); 
        
        yield return new WaitForSeconds(Smoke_Delay); //la duree avant l'explosion de la bombe

        position = bomb.transform.position;
        //cree SmokeExplosion dans la position de SmokeBomb
        GameObject explosion = Instantiate(SmokeExplosion, 
            position, 
            Quaternion.identity);
        //Envoie la requete d'explosion de la bombe
        pv.RPC("ExplodeSmoke", RpcTarget.Others, position, (float) PhotonNetwork.Time);  
        Destroy(bomb);
        
        yield return new WaitForSeconds(Smoke_Spell_Duration); //duree d'emission de la smoke
        
        Destroy(explosion);
        
        yield return new WaitForSeconds(Smoke_Cooldown - Smoke_Spell_Duration - Smoke_Delay);//duree du cooldown
        
        Can_Smoke = true;
    }

    [PunRPC]
    //Requete de spawn de la bombe
    public void SpawnSmoke(Vector3 position, Vector3 direction, float sendMoment)
    {
        float latency = (float) (PhotonNetwork.Time - sendMoment);

        GameObject bomb = Instantiate(SmokeBomb,
            position + direction * latency + 0.5f*latency*latency*Physics.gravity,
            Quaternion.identity);
        
        bomb.GetComponent<Rigidbody>().AddForce(direction*1000 + latency*Physics.gravity); //Applique une force

        Destroy(bomb, Smoke_Delay-latency);
    }

    [PunRPC]
    //Requete d'explosion de la bombe
    public void ExplodeSmoke(Vector3 position, float sendMoment)
    {
        float latency = (float) (PhotonNetwork.Time - sendMoment);
        
        GameObject explosion = Instantiate(SmokeExplosion, 
            position, 
            Quaternion.identity);
        
        Destroy(explosion, Smoke_Spell_Duration-latency);
    }
}
