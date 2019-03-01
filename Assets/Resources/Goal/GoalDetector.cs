using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class GoalDetector : MonoBehaviour
{
    [SerializeField] public Team team = Team.Blue;             //La team qui doit marquer dans ce but

    private void OnTriggerEnter(Collider other)
    {
        //La balle a le tag Ball (vous pouvez gerer les tags dans le menu de la prefab en haut)
        if (PhotonNetwork.IsMasterClient && other.CompareTag("Ball"))
            Goal(other.gameObject);
    }

    private void Goal(GameObject ball)
    {
        GetComponent<GoalExplosion>().MakeGoalExplosion(ball.transform.position);
        
        //Informe le GameManager du but
        GameManagerHost.OnGoal(team == Team.Blue);
    }
}