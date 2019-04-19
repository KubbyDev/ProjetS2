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
    
    private bool CanFreeze = true;   //Vrai si le cooldown du freeze est termine;

    public void Freeze()
    {
        if (!CanFreeze)
            return;
        
        Vector3 position = transform.position + new Vector3(0, 1, 0) + transform.forward;
        Quaternion rotation = GetComponent<PlayerInfo>().cameraAnchor.rotation;
        
        //cree et prend la position de la ball en fonction de la camera
        Instantiate(FreezeBall, position, rotation);
        
        GetComponent<PhotonView>().RPC("SpawnFreeze", RpcTarget.Others, position, rotation, (float) PhotonNetwork.Time);

        //Lance le cooldown
        StartCoroutine(FreezeCoroutine());
    }

    IEnumerator FreezeCoroutine()
    {
        CanFreeze = false;
        yield return new WaitForSeconds(Freeze_Cooldown);
        CanFreeze = true;
    }

    [PunRPC]
    public void SpawnFreeze(Vector3 position, Quaternion rotation, float sendMoment)
    {
        float latency = (float) (PhotonNetwork.Time - sendMoment);
        
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
    
    private bool MagnetOffCooldown = true;      // Indicateur en cooldown
    
    public void MagnetSpell()
    {
        if(!MagnetOffCooldown)  
            StartCoroutine(MagnetCoroutine());
    }

    IEnumerator MagnetCoroutine()
    {
        Info.maxCatchRange += MagnetBonusRange;                         // Application du bonus de range
        MagnetOffCooldown = false;                                      // Le spell passe en cooldown
        yield return new WaitForSeconds(MagnetSpellDuration);           // Duree du bonus
        Info.maxCatchRange -= MagnetBonusRange;                         // Retour a la normale de la range
        yield return new WaitForSeconds(MagnetCooldown);                // Duree du cooldown
        MagnetOffCooldown = true;                                       // Le spell redevient utilisable
    }
}
