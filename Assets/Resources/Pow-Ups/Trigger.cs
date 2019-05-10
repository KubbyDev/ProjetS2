using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    [SerializeField] public int cooldown;
    [SerializeField] private int use;
    
    private bool exists = false;
    private float timer;

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
        bool alreadyHadPU = false;
        
        if (other.gameObject.CompareTag("Player"))
        {
            switch (use)
            {
                case 1:
                {
                    alreadyHadPU = other.GetComponent<Back>().Player_Has_Back;
                    other.GetComponent<Back>().Player_Got_Back();
                    break;
                }

                case 2:
                {
                    alreadyHadPU = other.GetComponent<Hook>().Player_Has_Hook;
                    other.GetComponent<Hook>().Player_Got_Hook();
                    break;
                }

                case 3:
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
        }
    }
}
