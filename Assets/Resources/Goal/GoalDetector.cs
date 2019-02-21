using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;

public class GoalDetector : MonoBehaviour
{
    [SerializeField] public PlayerInfo.Team team = PlayerInfo.Team.Blue;  //La team qui doit marquer dans ce but
    [SerializeField] private ParticleSystem goalExplosion;                //Reference a la particule de but

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
        GameObject.Find("GameManager").GetComponent<GameManager>().OnGoal(team == PlayerInfo.Team.Blue);
        
        //Incremente le nombre de buts marques du joueur qui a marque
        ball.GetComponent<Ball>().shooter.GetComponent<PlayerInfo>().goalsScored++;
    }
}
