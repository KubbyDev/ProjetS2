using Photon.Pun;
using UnityEngine;

//Cette classe detecte que la balle entre dans le but

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
            //Si la partie a demarre, il faut informer les clients qu'il y a eu un but
            
            //Si ce n'est pas le Host on ne fait rien
            if (!PhotonNetwork.IsMasterClient) 
                return;
            
            //Si c'est le host on informe le GameManagerHost du but, pour qu'il transmette l'info
            GameManagerHost.OnGoal(team == Team.Orange, ballPosition);
        }
        else
            //Si la partie n'a pas demarre on fait juste une explosion de but
            GetComponent<GoalExplosion>().MakeGoalExplosion(ballPosition);
    }
}