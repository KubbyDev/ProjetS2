using Photon.Pun;
using System.Collections;
using UnityEngine;

public class BasicSpell : MonoBehaviour
{
    [SerializeField] private GameObject BasicSpellbullet;
    [SerializeField] private float BasicSpellCooldown = 15f;
    [SerializeField] private float BulletLifeTime = 3f;

    private bool BasicSpellOffCooldown = true;

    private PlayerInfo playerinfocaster;
    private PhotonView pv;

    void Start()
    {
        playerinfocaster = GetComponent<PlayerInfo>();
        pv = GetComponent<PhotonView>();
    }

    public void Basic_Spell()
    {
        if (!BasicSpellOffCooldown) 
            return;

        Vector3 position = transform.position + new Vector3(0, 0.5f, 0) + Vector3.forward*1.0f;
        Vector3 direction = playerinfocaster.cameraAnchor.forward;
        
        GameObject bullet = Instantiate(BasicSpellbullet, position, Quaternion.identity);
       
        //Informe les autres clients
        pv.RPC("SpawnBasicBullet", RpcTarget.Others, position, direction);   
        
        Destroy(bullet, BulletLifeTime);
        bullet.GetComponent<BasicSpellBall>().Init(direction, true);
            
        StartCoroutine(BasicSpellCoroutine());
    }

    IEnumerator BasicSpellCoroutine()
    {
        BasicSpellOffCooldown = false;
        yield return new WaitForSeconds(BasicSpellCooldown);
        BasicSpellOffCooldown = true;
    }

    [PunRPC]
    public void SpawnBasicBullet(Vector3 position, Vector3 direction, PhotonMessageInfo info)
    {
        float latency = Tools.GetLatency(info.timestamp);
        
        GameObject bullet = Instantiate(BasicSpellbullet,
            position + latency*direction,
            Quaternion.identity);
       
        Destroy(bullet, BulletLifeTime - latency);
        bullet.GetComponent<BasicSpellBall>().Init(direction, false);
    }
}