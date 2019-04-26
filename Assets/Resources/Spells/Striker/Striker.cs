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
    
    // TURBO -----------------------------------------------------------------------------------------------------------
    
    [SerializeField] private float speedDuration = 3f;        //Duree du speed
    [SerializeField] private float speedCooldown = 6f;        //Cooldown du speed
    [SerializeField] private float speedMultiplier = 1.5f;    //Force du speed

    public void Speed()
    {
        if (infos.firstCooldown > 0f) 
            return;
        
        infos.firstCooldown = speedCooldown;
        movement.MultiplySpeed(speedMultiplier, speedDuration);  //Augmente la vitesse pendant la duree du spell
    }
    
    // ESCAPE ----------------------------------------------------------------------------------------------------------

    [SerializeField] private float escapeCooldown = 3f;       //Cooldown du escape
    [SerializeField] private GameObject escapeBullet;         //Prefab de la balle pour escape
    
    public void Escape()
    {
        if (infos.secondCooldown > 0f) //secondCooldown = cooldown du E = cooldown de escape
            return;
        
        Vector3 position = transform.position + new Vector3(0, 1.5f, 0) + transform.forward*1.0f;
        Vector3 direction = infos.cameraRotation * Vector3.forward;
        
        //Cree escapeBullet
        GameObject bullet = Instantiate(escapeBullet, position, Quaternion.identity);
        
        pv.RPC("SpawnEscape", RpcTarget.Others, position, direction);
        
        bullet.GetComponent<Rigidbody>().AddForce(direction * 1000);  //Applique une force
        bullet.GetComponent<TeleportBullet>().Init(this.gameObject, Time.time, true);  //Donne a la balle une reference au joueur qu'elle va devoir tp

        infos.secondCooldown = escapeCooldown;
    }

    [PunRPC]
    public void SpawnEscape(Vector3 position, Vector3 direction, PhotonMessageInfo info)
    {
        float latency = Tools.GetLatency(info.timestamp);
        
        GameObject bullet = Instantiate(escapeBullet,
            position + latency*direction,
            Quaternion.identity);
                
        bullet.GetComponent<Rigidbody>().AddForce(direction*1000);  //Applique une force
        bullet.GetComponent<TeleportBullet>().Init(null, 0, false);
    }
}
