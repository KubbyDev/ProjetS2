using Photon.Pun;
using System.Collections;
using Photon.Realtime;
using UnityEngine;

public class BasicSpell : MonoBehaviour
{
    [SerializeField] private GameObject BasicSpellbullet;
    [SerializeField] private float BasicSpellCooldown = 15f;
    [SerializeField] private float BulletLifeTime = 3f;

    private PlayerInfo playerinfocaster;
    private PhotonView pv;

    void Start()
    {
        playerinfocaster = GetComponent<PlayerInfo>();
        pv = GetComponent<PhotonView>();
    }

    public void Basic_Spell()
    {
        if (playerinfocaster.BACooldown > 0) 
            return;

        Vector3 position = transform.position + new Vector3(0, 0.5f, 0) + transform.forward*1.0f;
        Vector3 direction = playerinfocaster.cameraRotation * Vector3.forward;
        
        GameObject bullet = Instantiate(BasicSpellbullet, position, Quaternion.identity);
       
        //Informe les autres clients
        pv.RPC("SpawnBasicBullet", RpcTarget.Others, position, direction);   
        
        Destroy(bullet, BulletLifeTime);
        bullet.GetComponent<BasicSpellBall>().Init(direction, true, this.gameObject);
            
        playerinfocaster.BACooldown = BasicSpellCooldown;
    }

    [PunRPC]
    public void SpawnBasicBullet(Vector3 position, Vector3 direction, PhotonMessageInfo info)
    {
        float latency = Tools.GetLatency(info.timestamp);
        
        GameObject bullet = Instantiate(BasicSpellbullet,
            position + latency*direction,
            Quaternion.identity);
       
        Destroy(bullet, BulletLifeTime - latency);
        bullet.GetComponent<BasicSpellBall>().Init(direction, false, this.gameObject);
    }
}