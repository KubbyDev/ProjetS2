using System.Collections;
using Photon.Pun;
using UnityEngine;

public class Striker : MonoBehaviour
{
    [SerializeField] private float TimeSpeedSpell = 3f;        //Duree du speed
    [SerializeField] private float TimeSpeedCooldown = 3f;     //Cooldown du spell
    [SerializeField] private float speedMultiplier = 1.5f;     //Force du speed
    [SerializeField] private GameObject escapeBullet;          //Prefab de la balle pour escape

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
            yield return new WaitForSeconds(TimeSpeedSpell);    //La duree du spell
            movement.MultiplySpeed(1 / speedMultiplier);        //Remet la vitesse normale
            yield return new WaitForSeconds(TimeSpeedCooldown); //La duree du cooldown
            canSpeed = true;
        }
    }

    public void escape()
    {
        if (!canEscape)
            return;
        
        Vector3 position = transform.position + new Vector3(0, 1.5f, 0) + transform.forward*1.0f;
        Vector3 direction = infos.cameraAnchor.forward;
        
        //Cree escapeBullet
        GameObject bullet = Instantiate(escapeBullet, position, Quaternion.identity);
        
        pv.RPC("SpawnEscape", RpcTarget.Others, position, direction);
        
        bullet.GetComponent<Rigidbody>().AddForce(direction * 1000);  //Applique une force
        bullet.GetComponent<TeleportBullet>().SetShooter(this.gameObject);  //Donne a la balle une reference au joueur qu'elle va devoir tp
        
        StartCoroutine(EscapeCouroutine()); //Lance la coroutine 
    }

    IEnumerator EscapeCouroutine()
    {
        canEscape = false;
        yield return new WaitForSeconds(TimeSpeedCooldown); //la duree du cooldown
        canEscape = true;
    }

    [PunRPC]
    public void SpawnEscape(Vector3 position, Vector3 direction, PhotonMessageInfo info)
    {
        float latency = (float) (PhotonNetwork.Time - info.timestamp);
        
        GameObject bullet = Instantiate(escapeBullet,
            position + latency*direction + 0.5f*latency*latency*Physics.gravity,
            Quaternion.identity);
                
        bullet.GetComponent<Rigidbody>().AddForce(direction * 1000);  //Applique une force
        bullet.GetComponent<TeleportBullet>().SetShooter(this.gameObject);
    }
}
