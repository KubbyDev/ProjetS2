using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Freeze : MonoBehaviour
{
   [SerializeField] private float vitesseBall = 5f;
   
    void Update()
    {
        transform.position += transform.forward * Time.deltaTime * vitesseBall; //envoie la ball dans la direction de la camera
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            other.GetComponent<Ball>().FreezeBall();
            
        }
    }
}
