﻿using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

//Ce script gere les interaction entre le joueur et la balle

public class BallManager : MonoBehaviour
{
    public const float launchStrength = 2;        //La force avec laquelle la balle est jetee
    public const float catchCooldown = 1;         //Le temps entre 2 tentative pour attraper la balle

    public bool hasBall = false;                        //Si le joueur a la balle
    public float catchTimeLeft = 0;                     //Le temps restant avant de pouvoir reutiliser le catch
    
    private PlayerInfo infos;                           //Le script qui contient les infos sur le joueur
    private PhotonView pv;                              //Le script qui gere ce joueur sur le reseau
    private float PowerShootTimeLeft = 0;               //Le temps restant pour utiliser le powershoot
    
    void Start()
    {
        infos = GetComponent<PlayerInfo>();
        pv = GetComponent<PhotonView>();
    }

    //Recuperation de la balle
    public void Catch()
    {
        if (!CanCatch())
            return;
            
        //On enleve la possession de balle sur tous les joueurs et on la donne au joueur qui vient de la recuperer
        Ball.UpdatePossessor(this.gameObject);

        catchTimeLeft = catchCooldown;
    }

    public bool CanCatch()
    {
        if (catchTimeLeft > 0 || !Ball.script.canBeCaught) 
            return false;
        
        //On regarde si la balle est devant la camera a une distance inferieure a maxCatchDistance
        foreach (RaycastHit hit in Physics.SphereCastAll(infos.cameraPosition, infos.catchWidth, infos.cameraRotation*Vector3.forward, infos.maxCatchRange))
            //On recupere la balle si on la touche ou si on touche son porteur
            if (hit.collider.CompareTag("Ball") || hit.collider.CompareTag("Player") &&
                hit.collider.gameObject.GetComponent<BallManager>().hasBall)
            {
                return true;   
            }

        return false;
    }
    
    void Update()
    {
        infos.hasBall = hasBall;

        if (catchTimeLeft > 0)                 //Met a jour le temps restant pour attraper la balle
            catchTimeLeft -= Time.deltaTime;
        if (hasBall)                           //Si le joueur a la balle, on met son cooldown au max
            catchTimeLeft = catchCooldown;     //pour qu'il ne puisse pas la recuperer instant quand on lui prend
        
        if (PowerShootTimeLeft > 0)            //Met a jour le temps restant pour utiliser le powershoot
            PowerShootTimeLeft -= Time.deltaTime;
        
    }

    //Lance la balle devant lui
    public void Shoot()
    {
        if (!hasBall)
            return;
            
        //Si le joueur peut utiliser powershoot, on applique le multiplieur, sinon non
        Ball.script.Shoot(infos.cameraRotation * Vector3.forward * launchStrength * 1000 * (PowerShootTimeLeft>0 ? PowerShoot.multiplier : 1), 
            PowerShootTimeLeft>0);  //Vrai si le joueur a utilise le powershoot, faux sinon
    }                                       

    public void Use_PowerShoot()
    {
        pv.RPC("Use_PowerShoot_RPC", RpcTarget.Others, PhotonNetwork.Time);
       
        PowerShootTimeLeft = PowerShoot.cooldown; //Indique au script que le joueur a utilise le powerup powershoot
        ParticleSystem.MainModule Flames = this.transform.Find("FlamesParticles").GetComponent<ParticleSystem>().main;
        Flames.duration = PowerShootTimeLeft;
        this.transform.Find("FlamesParticles").GetComponent<ParticleSystem>().Play();
    }
    
    [PunRPC]
    public void Use_PowerShoot_RPC(double sendMoment)
    {
        PowerShootTimeLeft = PowerShoot.cooldown - Tools.GetLatency(sendMoment); //Indique au script que le joueur a utilise le powerup powershoot
        ParticleSystem.MainModule Flames = this.transform.Find("FlamesParticles").GetComponent<ParticleSystem>().main;
        Flames.duration = PowerShootTimeLeft;
        this.transform.Find("FlamesParticles").GetComponent<ParticleSystem>().Play();
    }

    public float GetLaunchSpeed()
    {
        return (
                   infos.cameraRotation * Vector3.forward * Time.fixedDeltaTime * launchStrength * 1000 * (PowerShootTimeLeft > 0 ? PowerShoot.multiplier : 1)
                   + Ball.rigidBody.velocity
                ).magnitude
               / (Ball.rigidBody.mass * 1.05f); //Le 1.05 permet de prendre a peu pres en compte la resistance de l'air
    }
}
