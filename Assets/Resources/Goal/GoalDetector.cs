using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalDetector : MonoBehaviour
{
    [SerializeField] public int team = 1;   //1 = les oranges doivent marquer dedans, -1 = les bleus doivent marquer dedans

    private void OnTriggerEnter(Collider other)
    {
        //La balle a le tag Ball (vous pouvez gerer les tags dans le menu de la prefab en haut)
        if (other.tag == "Ball")
            Goal();
    }

    private void Goal()
    {
        Debug.Log("GOAL for " + team);

    }
}
