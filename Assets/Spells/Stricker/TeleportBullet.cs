using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportBullet : MonoBehaviour    
{
    private GameObject shooter;    //Reference au joueur

    void Update()
    {
        //Quand la balle arrive a la fin de son chemin
        if ((transform.position - shooter.transform.position).magnitude > 15)
        {
            shooter.transform.position = transform.position;
            Destroy(this.gameObject);
        }
    }

    public void SetShooter(GameObject pShooter)
    {
        shooter = pShooter;
    }
}

    
