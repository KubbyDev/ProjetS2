using System.Collections;
using Photon.Pun;
using UnityEngine;

public class Striker : MonoBehaviour
{
    [SerializeField] private float speedDuration = 3f;        //Duree du speed
    [SerializeField] private float speedCooldown = 3f;        //Cooldown du speed
    [SerializeField] private float speedMultiplier = 1.5f;    //Force du speed
    
    [SerializeField] private float escapeCooldown = 3f;       //Cooldown du escape
    [SerializeField] private GameObject escapeBullet;         //Prefab de la balle pour escape

    private MovementManager movement;          //Reference au script qui gere les mouvements du joueur
    private PlayerInfo infos;                  //Reference au script qui contient des infos sur le joueur
    private PhotonView pv;                     //Le script qui gere ce gameobject sur le reseau
    private bool canSpeed = true;              //canSpeed = cooldown de Turbo fini
    private bool canEscape = true;             //canEscape = cooldown de Escape fini

    void Start()
    {
        movement = GetComponent<MovementManager>();
        infos = GetComponent<PlayerInfo>();
        pv = GetComponent<PhotonView>();
    }

    public void Speed()
    {
        StartCoroutine(SpeedCouroutine());  //Lance la coroutine  
    }

    IEnumerator SpeedCouroutine()
    {
        if (canSpeed) //canSpeed = cooldown est fini
        {
            movement.MultiplySpeed(speedMultiplier);            //Multiplie la vitesse
            canSpeed = false;
            yield return new WaitForSeconds(speedDuration);     //La duree du spell
            movement.MultiplySpeed(1 / speedMultiplier);        //Remet la vitesse normale
            yield return new WaitForSeconds(speedCooldown);     //La duree du cooldown
            canSpeed = true;
        }
    }

    public void Escape()
    {
        if (!canEscape)
            return;
        
        Vector3 position = transform.position + new Vector3(0, 1.5f, 0) + transform.forward*1.0f;
        Vector3 direction = infos.cameraAnchor.forward;
        
        //Cree escapeBullet
        GameObject bullet = Instantiate(escapeBullet, position, Quaternion.identity);
        
        pv.RPC("SpawnEscape", RpcTarget.Others, position, direction);
        
        bullet.GetComponent<Rigidbody>().AddForce(direction * 1000);  //Applique une force
        bullet.GetComponent<TeleportBullet>().Init(this.gameObject, Time.time, true);  //Donne a la balle une reference au joueur qu'elle va devoir tp
        
        StartCoroutine(EscapeCouroutine()); //Lance la coroutine 
    }

    IEnumerator EscapeCouroutine()
    {
        canEscape = false;
        yield return new WaitForSeconds(escapeCooldown); //la duree du cooldown
        canEscape = true;
    }

    [PunRPC]
    public void SpawnEscape(Vector3 position, Vector3 direction, PhotonMessageInfo info)
    {
        float latency = (float) (PhotonNetwork.Time - info.timestamp);
        
        GameObject bullet = Instantiate(escapeBullet,
            position + latency*direction,
            Quaternion.identity);
                
        bullet.GetComponent<Rigidbody>().AddForce(direction*1000);  //Applique une force
        bullet.GetComponent<TeleportBullet>().Init(null, 0, false);
    }
}
