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
            exists = true;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            switch (use)
            {
                case 1:
                {
                    other.GetComponent<Back>().Player_Got_Back();
                    break;
                }

                case 2:
                {
                    other.GetComponent<Hook>().Player_Got_Hook();
                    break;
                }

                case 3:
                {
                    other.GetComponent<PowerShoot>().Player_Got_PowerShoot();
                    break;
                }
            }
            
            transform.GetChild(0).gameObject.SetActive(false);
            exists = false;
            timer = cooldown;
        }
    }
}
