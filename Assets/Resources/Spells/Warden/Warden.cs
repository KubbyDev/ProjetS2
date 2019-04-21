using System.Collections;
using Photon.Pun;
using UnityEngine;

public class Warden : MonoBehaviour
{
    private PlayerInfo Info;        // Informations du joueur

    void Start()                                                            
    {
        Info = GetComponent<PlayerInfo>();    // Recuperation des informations du joueur
    }
    
    // FREEZE ----------------------------------------------------------------------------------------------------------
    
    [SerializeField] private float Freeze_Cooldown = 20f;  //Duree du cooldown
    [SerializeField] private GameObject FreezeBall;        //Prefab de la FreezeBall

    public void Freeze()
    {
        if (Info.firstCooldown > 0f) //firstCooldown = cooldown du A = cooldown de Freeze
            return;
        
        Vector3 position = transform.position + new Vector3(0, 1, 0) + transform.forward;
        Quaternion rotation = GetComponent<PlayerInfo>().cameraAnchor.rotation;
        
        //cree et prend la position de la ball en fonction de la camera
        Instantiate(FreezeBall, position, rotation);
        
        GetComponent<PhotonView>().RPC("SpawnFreeze", RpcTarget.Others, position, rotation, PhotonNetwork.Time);

        //Lance le cooldown
        Info.firstCooldown = Freeze_Cooldown;
    }

    [PunRPC]
    public void SpawnFreeze(Vector3 position, Quaternion rotation, double sendMoment)
    {
        float latency = Tools.GetLatency(sendMoment);
        
        GameObject ball = Instantiate(FreezeBall, 
            position + latency*(rotation*Vector3.forward), 
            rotation);

        //Le projectile est gere sur le client qui l'a lance
        ball.GetComponent<SphereCollider>().enabled = false;
    }
    
    // MAGNET ----------------------------------------------------------------------------------------------------------
    
    [SerializeField] private float MagnetSpellDuration = 5f;                // Duree du bonus de range
    [SerializeField] private float MagnetCooldown = 20f;                    // Cooldown du Magnet
    [SerializeField] private float MagnetBonusRange = 4f;                   // Bonus de range
    
    public void MagnetSpell()
    {
        if (Info.secondCooldown <= 0f) //secondCooldown = cooldown du E = cooldown de Magnet
            StartCoroutine(MagnetCoroutine());
    }

    IEnumerator MagnetCoroutine()
    {
        Info.secondCooldown = MagnetCooldown;
        Info.maxCatchRange += MagnetBonusRange;                         // Application du bonus de range
        yield return new WaitForSeconds(MagnetSpellDuration);           // Duree du bonus
        Info.maxCatchRange -= MagnetBonusRange;                         // Retour a la normale de la range
    }
}
