using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stricker : MonoBehaviour
{
    [SerializeField] private float TimeSpeedSpell = 3f;        //Le temps que le speed dure
    [SerializeField] private float TimeSpeedCooldown = 3f;     //Cooldown du spell
    [SerializeField] private float speedMultiplier = 1.5f;     //Force du speed
    [SerializeField] private GameObject escapeBullet;          //Prefab de la balle pour escape

    private MovementManager movement;
    private CameraManager cam;
    private bool canSpeed = true;
    private bool canEscape = true;

    void Start()
    {
        movement = GetComponent<MovementManager>();
        cam = GetComponent<CameraManager>();
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
        StartCoroutine(escapeCouroutine()); //Lance la coroutine 
    }

    IEnumerator escapeCouroutine()
    {
        if (canEscape)
        {
            //Cree escapeBullet
            GameObject bullet = Instantiate(escapeBullet, transform.position + new Vector3(0, 1.5f, 0) + transform.forward, cam.GetRotation());
            bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * 1000);  //Applique une force
            bullet.GetComponent<TeleportBullet>().SetShooter(this.gameObject);
            //escape(); //multiplie la vitess
            canEscape = false;
            yield return new WaitForSeconds(TimeSpeedCooldown); //la duree du cooldown
            canEscape = true;
        }
    }
}
