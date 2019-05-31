﻿using System.Collections;
using Photon.Pun;
using UnityEngine;

public class Warden : MonoBehaviour
{
    private PlayerInfo Info;        // Informations du joueur

    void Start()                                                            
    {
        Info = GetComponent<PlayerInfo>();    // Recuperation des informations du joueur
    }

    public void StopSpells()
    {
        StopCoroutine(MagnetCoroutine());
        Info.maxCatchRange = PlayerInfo.baseCatchRange;
    }
    
    // FREEZE ----------------------------------------------------------------------------------------------------------
    
    public const float Freeze_Cooldown = 20f;  //Duree du cooldown
    
    [SerializeField] private GameObject FreezeBall;        //Prefab de la FreezeBall

    public void Freeze()
    {
        if (Info.firstCooldown > 0f) //firstCooldown = cooldown du A = cooldown de Freeze
            return;
        
        Vector3 position = transform.position + new Vector3(0,1,0) + transform.forward;
        Quaternion rotation = Info.cameraRotation;
        
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
    
    public const float MagnetSpellDuration = 3f;                // Duree du bonus de range
    public const float MagnetCooldown = 20f;                    // Cooldown du Magnet
    public const float MagnetBonusRange = 10f;                  // Bonus de range
    
    public void MagnetSpell()
    {
        if (Info.secondCooldown <= 0f) //secondCooldown = cooldown du E = cooldown de Magnet
        {
            StartCoroutine(MagnetCoroutine());
            
            ParticleSystem.MainModule main = transform.Find("ElectricParticles").GetComponent<ParticleSystem>().main;
            main.duration = MagnetSpellDuration;
            main.startColor = GetComponent<PlayerInfo>().team.GetMaterial().color;
            transform.Find("ElectricParticles").GetComponent<ParticleSystem>().Play();
        }
    }

    IEnumerator MagnetCoroutine()
    {
        Info.secondCooldown = MagnetCooldown;
        Info.maxCatchRange += MagnetBonusRange;                         // Application du bonus de range
        yield return new WaitForSeconds(MagnetSpellDuration);           // Duree du bonus
        Info.maxCatchRange -= MagnetBonusRange;                         // Retour a la normale de la range
    }
}
