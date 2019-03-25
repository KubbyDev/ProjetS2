using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    [SerializeField] private int use;
    
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Hahahahahah");
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
            }
        }
    }
}
