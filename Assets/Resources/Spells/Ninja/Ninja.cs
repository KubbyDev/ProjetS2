using Photon.Pun;
using System.Collections;
using Photon.Realtime;
using UnityEngine;

public class Ninja : MonoBehaviour
{
    private MovementManager move;                //Reference au script qui gere les mouvements du joueur
    private PhotonView pv;                       //Le script qui gere cet objet sur le reseau
    private PlayerInfo info;                     //Reference au script qui gere la camera du joueur

    void Start()
    {
        move = GetComponent<MovementManager>();
        info = GetComponent<PlayerInfo>();
        pv = GetComponent<PhotonView>();
    }
    
    public void StopSpells()
    {
        StopCoroutine(ExplodeCoroutine());
        StopCoroutine(SmokeCoroutine());
    }
    
    // EXPLODE ---------------------------------------------------------------------------------------------------------
    
    public const float Explode_Spell_Duration = 1f;   //Duree du speed
    public const float Explode_Cooldown = 20f;        //Cooldown du spell
    public const float Explode_Speed_Boost = 3f;      //Force du speed
    public const float Explosion_Radius = 20.0f;      //Rayon dans lequel les joueurs subissent l'explosion
    public const float Explosion_Power = 50f;         //Puissance de l'explosion

    public void Explode_Spell()
    {
        if (info.firstCooldown <= 0f) //firstCooldown = cooldown du A = cooldown de Explode
        {
            StartCoroutine(ExplodeCoroutine());
            
            ParticleSystem.MainModule main = transform.Find("SpeedParticle").GetComponent<ParticleSystem>().main;
            main.duration = Explode_Spell_Duration;
            main.startColor = new ParticleSystem.MinMaxGradient(Tools.SetAlpha(GetComponent<PlayerInfo>().team.GetMaterial().color, 0.02f));
            transform.Find("SpeedParticle").GetComponent<ParticleSystem>().Play();
            
            pv.RPC("LaunchExplode_RPC", RpcTarget.Others, PhotonNetwork.Time);
        }
    }

    [PunRPC]
    public void LaunchExplode_RPC(double sendMoment)
    {
        ParticleSystem.MainModule main = transform.Find("SpeedParticle").GetComponent<ParticleSystem>().main;
        main.duration = Explode_Spell_Duration - Tools.GetLatency(sendMoment);
        main.startColor = new ParticleSystem.MinMaxGradient(Tools.SetAlpha(GetComponent<PlayerInfo>().team.GetMaterial().color, 0.02f));
        transform.Find("SpeedParticle").GetComponent<ParticleSystem>().Play();
    }

    IEnumerator ExplodeCoroutine()
    {
        info.firstCooldown = Explode_Cooldown;
        move.MultiplySpeed(Explode_Speed_Boost, Explode_Spell_Duration);  // Augmente la vitesse pendant un temps donne
        yield return new WaitForSeconds(Explode_Spell_Duration);
        Explosion();
    }

    private void Explosion()
    {
        //Renvoie la liste des joueurs presents sur le terrain
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        //Pour chaque joueur
        foreach(GameObject player in players)
        {
            float distance = (player.transform.position - this.transform.position).magnitude;
            
            //Si le joueur est trop proche, et que ce n'est pas celui qui a lance le spell
            if (player != this.gameObject &&
                player.GetComponent<PlayerInfo>().team.IsOpponnentOf(info.team) &&
                distance <= Explosion_Radius)
            {
                //On le propulse
                Vector3 Blast = (new Vector3(player.transform.position.x - transform.position.x, 0f, player.transform.position.z - transform.position.z).normalized + new Vector3(0,1.0f,0))
                                * (1 - distance/Explosion_Radius);
                
                player.GetComponent<MovementManager>().AddForce(Blast * Explosion_Power);
            }
        }
    }

    // SMOKE -----------------------------------------------------------------------------------------------------------
    
    public const float Smoke_Spell_Duration = 9.5f;     //Duree d'emission de la smoke            
    public const float Smoke_Cooldown = 15f;          //Cooldown du spell smoke
    public const float Smoke_Delay = 1f;              //Duree de l'existence de SmokeBomb avant d'exploser
    public const int networkIdentifier = 2;
    
    [SerializeField] private GameObject SmokeBomb;                //Prefab de la bombe pour le smoke
    [SerializeField] private GameObject SmokeExplosion;           //Prefab du ParticleSystem
    
    public void Smoke()
    {
        if (info.secondCooldown <= 0f) //secondCooldown = cooldown du E = cooldown de Smoke
            StartCoroutine(SmokeCoroutine());
    }

    IEnumerator SmokeCoroutine()
    {
        info.secondCooldown = Smoke_Cooldown;
        
        Vector3 position = transform.position + new Vector3(0,1,0) + transform.forward * 1.0f;
        Vector3 direction = info.cameraRotation * Vector3.forward;
        
        //Envoie la requete de spawn de la bombe
        pv.RPC("SpawnSmoke", RpcTarget.Others, position, direction, PhotonNetwork.Time);

        //Cree SmokeBomb
        GameObject bomb = Instantiate(SmokeBomb, 
            position, 
            Quaternion.identity);
        
        //Applique une force
        bomb.GetComponent<Rigidbody>().AddForce(direction * 2000, ForceMode.Acceleration);
        
        //Met en place le composant qui gere le projectile sur le reseau
        PhotonView view = bomb.AddComponent<PhotonView>();
        view.ViewID = pv.ViewID + networkIdentifier;

        yield return new WaitForSeconds(Smoke_Delay); //la duree avant l'explosion de la bombe

        position = bomb.transform.position;
        //cree SmokeExplosion dans la position de SmokeBomb
        GameObject explosion = Instantiate(SmokeExplosion, 
            position, 
            Quaternion.identity);
        //Envoie la requete d'explosion de la bombe
        pv.RPC("ExplodeSmoke", RpcTarget.Others, position, PhotonNetwork.Time);  
        
        Destroy(bomb);
        Destroy(explosion, Smoke_Spell_Duration);  //duree d'emission de la smoke
    }

    [PunRPC]
    //Requete de spawn de la bombe
    public void SpawnSmoke(Vector3 position, Vector3 direction, double sendMoment)
    {
        float latency = Tools.GetLatency(sendMoment);

        GameObject bomb = Instantiate(SmokeBomb,
            position + direction * latency + 0.5f*latency*latency*Physics.gravity,
            Quaternion.identity);
        
        bomb.GetComponent<Rigidbody>().AddForce(direction*2000 + latency*Physics.gravity, ForceMode.Acceleration); //Applique une force

        //Met en place le composant qui gere le projectile sur le reseau
        PhotonView view = bomb.AddComponent<PhotonView>();
        view.ViewID = pv.ViewID + networkIdentifier;
        
        Destroy(bomb, Smoke_Delay-latency);
    }

    [PunRPC]
    //Requete d'explosion de la bombe
    public void ExplodeSmoke(Vector3 position, double sendMoment)
    {
        float latency = Tools.GetLatency(sendMoment);
        
        GameObject explosion = Instantiate(SmokeExplosion, 
            position, 
            Quaternion.identity);
        
        Destroy(explosion, Smoke_Spell_Duration-latency);
    }
}
