using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    public const int cooldown = 10;
    
    public PowerUp type;
    public float timer;
    
    private bool exists = false;

    private void Start()
    {
        timer = cooldown;
        transform.GetChild(0).gameObject.SetActive(false);
        GetComponent<Light>().enabled = false;
    }

    private void Update()
    {
        if (timer > 0)
            timer -= Time.deltaTime;
        if (timer < 0)
            HandlePrefab();
            
    }

    public void HandlePrefab()
    {
        if (!exists)
        {
            transform.GetChild(0).gameObject.SetActive(true);
            GetComponent<Light>().enabled = true;
            exists = true;
        }
    }

    public void OnTriggerEnter(Collider other) 
    {
        //Si ce n'est pas le joueur local on ne fait rien
        if (!(other.gameObject.CompareTag("Player") && other.GetComponent<PhotonView>().IsMine)) 
            return;

        //Si le powerup n'a pas spawn on ne fait rien
        if (timer > 0)
            return;
        
        bool alreadyHadPU = false;
            
        switch (type)
        {
            case PowerUp.Back:
            {
                alreadyHadPU = other.GetComponent<Back>().Player_Has_Back;
                other.GetComponent<Back>().Player_Got_Back();
                break;
            }

            case PowerUp.Hook:
            {
                alreadyHadPU = other.GetComponent<Hook>().Player_Has_Hook;
                other.GetComponent<Hook>().Player_Got_Hook();
                break;
            }

            case PowerUp.PowerShoot:
            {
                alreadyHadPU = other.GetComponent<PowerShoot>().Player_Has_PowerShoot;
                other.GetComponent<PowerShoot>().Player_Got_PowerShoot();
                break;
            }
        }

        //Empeche de recuperer un powerup si le joueur l'a deja
        if (alreadyHadPU)
            return;
            
        transform.GetChild(0).gameObject.SetActive(false);
        GetComponent<Light>().enabled = false;
        exists = false;
        timer = cooldown;
            
        GetComponent<PhotonView>().RPC("GetItem_RPC", RpcTarget.Others, PhotonNetwork.Time);
    }

    [PunRPC]
    public void GetItem_RPC(double sendMoment)
    {
        transform.GetChild(0).gameObject.SetActive(false);
        GetComponent<Light>().enabled = false;
        exists = false;
        timer = cooldown - Tools.GetLatency(sendMoment);
    }
}
