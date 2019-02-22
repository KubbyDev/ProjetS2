using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;

public class GoalDetector : MonoBehaviour
{
    [SerializeField] public Team team = Team.Blue;             //La team qui doit marquer dans ce but
    [SerializeField] private ParticleSystem goalExplosion;     //Reference a la particule de but

    private void OnTriggerEnter(Collider other)
    {
        //La balle a le tag Ball (vous pouvez gerer les tags dans le menu de la prefab en haut)
        if (other.CompareTag("Ball"))
            Goal(other.gameObject);
    }

    private void Goal(GameObject ball)
    {
        //Fait pop les particules de goal explosion
        Instantiate(goalExplosion, ball.transform.position, Quaternion.identity);
        
        //Informe le GameManager du but
        GameManager.script.OnGoal(team == Team.Blue);
        
        //Incremente le nombre de buts marques du joueur qui a marque
        Ball.script.shooter.GetComponent<PlayerInfo>().goalsScored++;
    }
}
