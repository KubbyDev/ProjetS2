using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class GoalDetector : MonoBehaviour
{
    public static GameObject[] goals;    //Une reference a chaque but (0: bleu, 1: orange)
    
    [SerializeField] public Team team;   //La team a laquelle ce but appartient

    void Awake()
    {
        if(goals == null)
            goals = new GameObject[2];
        
        //En 0 on met une reference au but bleu, en 1 au but orange
        goals[team == Team.Blue ? 0 : 1] = this.gameObject;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        //La balle a le tag Ball (vous pouvez gerer les tags dans le menu de la prefab en haut)
        if (other.CompareTag("Ball"))
            Goal(other.gameObject.transform.position);
    }

    private void Goal(Vector3 ballPosition)
    {
        if (GameManager.gameStarted)
        {
            //Si ce n'est pas le Host on ne fait rien
            if (!PhotonNetwork.IsMasterClient) 
                return;
            
            //Informe le GameManager du but
            GameManagerHost.OnGoal(team == Team.Orange);
            GameManager.script.OnGoal(team == Team.Orange, ballPosition);
        }
        else
            GetComponent<GoalExplosion>().MakeGoalExplosion(ballPosition);
    }
}