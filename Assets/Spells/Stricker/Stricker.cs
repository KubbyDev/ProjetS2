using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stricker : MonoBehaviour
{
    private MovementManager movement;
    [SerializeField]private float TimeSpeedSpell = 3f;
    [SerializeField] private float TimeSpeedCooldown = 8f;
    [SerializeField]private float speedMultiplier = 1.5f;
    private bool canSpeed = true ;
    [SerializeField] private GameObject escapeBullet;

    void Start()
    {
        movement = GetComponent<MovementManager>();
    }

    void Update()
    {
        
    }

    public void Speed()
    {
        StartCoroutine(SpeedCouroutine());  //Lance la coroutine  
    }

    IEnumerator SpeedCouroutine()
    {
        if (canSpeed) //canSpeed = cooldown est fini
        {
            movement.MultiplySpeed(speedMultiplier); //multiplie la vitess
            canSpeed = false;
            yield return new WaitForSeconds(TimeSpeedSpell); //la duree du spell
            movement.MultiplySpeed(1 / speedMultiplier); //remet la vitesse normale
            yield return new WaitForSeconds(TimeSpeedCooldown); //la duree du cooldown
            canSpeed = true;
        }   
    }

    public void escape()
    {
        GameObject _object = Instantiate(escapeBullet, transform.position + new Vector3(0, 1.5f, 0) + transform.forward, transform.rotation);
        _object.GetComponent<Rigidbody>().AddForce(transform.forward * 1000); //cree escapeBullet 
    }
}
