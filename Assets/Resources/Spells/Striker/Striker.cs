using System.Collections;
using Photon.Pun;
using UnityEngine;

public class Striker : MonoBehaviour
{
    private MovementManager movement;          //Reference au script qui gere les mouvements du joueur
    private PlayerInfo infos;                  //Reference au script qui contient des infos sur le joueur
    private PhotonView pv;                     //Le script qui gere ce gameobject sur le reseau
    
    void Start()
    {
        movement = GetComponent<MovementManager>();
        infos = GetComponent<PlayerInfo>();
        pv = GetComponent<PhotonView>();
    }
    
    public void StopSpells()
    {
    }
    
    // TURBO -----------------------------------------------------------------------------------------------------------
    
    public const float speedDuration = 3f;        //Duree du speed
    public const float speedCooldown = 6f;        //Cooldown du speed
    public const float speedMultiplier = 1.5f;    //Force du speed

    public void Speed()
    {
        if (infos.firstCooldown > 0f) 
            return;
        
        infos.firstCooldown = speedCooldown;
        movement.MultiplySpeed(speedMultiplier, speedDuration);  //Augmente la vitesse pendant la duree du spell
    }
    
    // ESCAPE ----------------------------------------------------------------------------------------------------------

    public const float escapeCooldown = 10f;       //Cooldown du escape
    public const float bulletSpeed = 20f;          //Vitesse de la balle de tp
    public const int networkIdentifier = 3;
    
    [SerializeField] private GameObject escapeBullet;         //Prefab de la balle pour escape
    
    public void Escape()
    {
        if (infos.secondCooldown > 0f) //secondCooldown = cooldown du E = cooldown de escape
            return;
        
        Vector3 position = transform.position + new Vector3(0,1,0);
        Vector3 direction = infos.cameraRotation * Vector3.forward;
        
        pv.RPC("SpawnEscape", RpcTarget.Others, position, direction);
        
        //Cree escapeBullet
        GameObject bullet = Instantiate(escapeBullet, position, Quaternion.identity);
        
        //Met en place le composant qui gere le projectile sur le reseau
        PhotonView view = bullet.AddComponent<PhotonView>();
        view.ViewID = pv.ViewID + networkIdentifier;

        bullet.GetComponent<Rigidbody>().AddForce(100 * bulletSpeed * direction);  //Applique une force
        
        //Donne a la balle une reference au joueur qu'elle va devoir tp
        bullet.GetComponent<TeleportBullet>().Init(this.gameObject, Time.time, true, direction);

        infos.secondCooldown = escapeCooldown;
    }

    [PunRPC]
    public void SpawnEscape(Vector3 position, Vector3 direction, PhotonMessageInfo info)
    {
        float latency = Tools.GetLatency(info.timestamp);
        
        GameObject bullet = Instantiate(escapeBullet,
            position + latency*direction,
            Quaternion.identity);
        
        //Met en place le composant qui gere le projectile sur le reseau
        PhotonView view = bullet.AddComponent<PhotonView>();
        view.ViewID = pv.ViewID + networkIdentifier;
        
        bullet.GetComponent<Rigidbody>().AddForce(direction*1000);  //Applique une force
        bullet.GetComponent<TeleportBullet>().Init(null, 0, false, direction);
    }
}
