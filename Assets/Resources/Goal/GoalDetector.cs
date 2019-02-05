using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalDetector : MonoBehaviour
{
    [SerializeField] public int team = 1;                     //1 = les oranges doivent marquer dedans, -1 = les bleus doivent marquer dedans
    [SerializeField] private ParticleSystem goalExplosion;

    private void OnTriggerEnter(Collider other)
    {
        //La balle a le tag Ball (vous pouvez gerer les tags dans le menu de la prefab en haut)
        if (other.tag == "Ball")
            Goal(other.gameObject);
    }

    private void Goal(GameObject ball)
    {
        //Fait pop les particules de goal explosion
        Instantiate(goalExplosion, ball.transform.position, Quaternion.identity);
    }
}
