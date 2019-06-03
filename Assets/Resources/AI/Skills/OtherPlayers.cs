using System;
using System.Linq;
using UnityEngine;

public partial class Skills
{
    /// <summary>
    /// Renvoie le joueur le plus proche de fromPosition parmis ceux qui satisfont la condition filter
    /// </summary>
    /// <param name="filter"></param>
    /// <param name="fromPosition"></param>
    /// <param name="ignoreSelf"></param>
    /// <returns></returns>
    public GameObject GetNearestPlayer(Func<GameObject, bool> filter, Vector3 fromPosition, bool ignoreSelf = false)
    {
        float minDist = float.PositiveInfinity;
        GameObject nearest = null;

        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player").Where(filter).Where(player => !(ignoreSelf && player == this.gameObject)))
        {
            float dist = Vector3.Distance(player.transform.position, fromPosition);
            if (dist < minDist)
            {
                nearest = player;
                minDist = dist;
            }
        }

        return nearest;
    }

    /// <summary>
    /// Renvoie l'adversaire le plus proche
    /// </summary>
    /// <returns></returns>
    public GameObject GetNearestOpponent()
    {
        return GetNearestPlayer(
            player => player.GetComponent<PlayerInfo>().team.IsOpponnentOf(infos.team),
            transform.position
        );  
    }
    
    /// <summary>
    /// Renvoie l'allie le plus proche
    /// </summary>
    /// <returns></returns>
    public GameObject GetNearestAlly()
    {
        return GetNearestPlayer(
            player => player.GetComponent<PlayerInfo>().team == infos.team,
            transform.position,
            true
        );  
    }

    /// <summary>
    /// Renvoie l'adversaire le plus proche des buts allies
    /// </summary>
    /// <returns></returns>
    public GameObject GetNearestOpponentFromAllyGoal()
    {
        return GetNearestPlayer(
            player => player.GetComponent<PlayerInfo>().team.IsOpponnentOf(infos.team),
            AllyGoal().transform.position
        );  
    }
    
    /// <summary>
    /// Renvoie l'allie le plus proche des buts allies
    /// </summary>
    /// <returns></returns>
    public GameObject GetNearestAllyFromAllyGoal()
    {
        return GetNearestPlayer(
            player => player.GetComponent<PlayerInfo>().team == infos.team,
            AllyGoal().transform.position
        );  
    }

    /// <summary>
    /// Renvoie le joueur le plus proche des buts allies
    /// </summary>
    /// <returns></returns>
    public GameObject GetNearestPlayerFromAllyGoal()
    {
        return GetNearestPlayer(player => player, AllyGoal().transform.position);
    }
    
    
    /// <summary>
    /// Renvoie l'adversaire le plus proche de la balle
    /// </summary>
    /// <returns></returns>
    public GameObject GetNearestOpponentFromEnemyGoal()
    {
        return GetNearestPlayer(
            player => player.GetComponent<PlayerInfo>().team.IsOpponnentOf(infos.team),
            EnemyGoal().transform.position
        );  
    }

    /// <summary>
    /// Renvoie l'allie le plus proche de la balle
    /// </summary>
    /// <returns></returns>
    public GameObject GetNearestAllyFromBall()
    {
        return GetNearestPlayer(player => player.GetComponent<PlayerInfo>().team == infos.team,
            Ball.ball.transform.position);
    }

    public GameObject GetNearestOpponentFromBall()
    {
        return GetNearestPlayer(player => player.GetComponent<PlayerInfo>().team.IsOpponnentOf(infos.team),
            Ball.ball.transform.position);
    }

    /// <summary>
    /// Renvoie l'allie le plus proche parmi ceux qui sont devant (plus proche du but)
    /// null si il n'y a pas d'allie devant
    /// </summary>
    /// <returns></returns>
    public GameObject GetNearestAllyInFront()
    {
        float distToGoal = EnnemyGoalDist();
        
        return GetNearestPlayer(
            player => player.GetComponent<PlayerInfo>().team == infos.team 
                      && GetHorizontalDistance(player.transform.position, EnemyGoal().transform.position) < distToGoal,
            transform.position,
            true
        );  
    }

    /// <summary>
    /// Renvoie vrai si il y a un defenseur entre le shootingPlayer et ses cages adverses
    /// </summary>
    public bool IsDefenderReady(GameObject shootingPlayer)
    {
        Vector3 spPosition = shootingPlayer.transform.position;
        PlayerInfo spInfos = shootingPlayer.GetComponent<PlayerInfo>();
        Vector3 enemyGoalPosition = (spInfos.team == infos.team ? EnemyGoal() : AllyGoal()).transform.position;
        float enemyGoalDist = Vector3.Distance(enemyGoalPosition, spPosition);
        
        return 
            //Sphere cast vers le but
            Physics.SphereCastAll(spPosition, 10, enemyGoalPosition - spPosition, enemyGoalDist)
                //Si un seul des elements touches est un adversaire, on renvoie true
            .Any(hit => hit.collider.CompareTag("Player") && 
                        hit.collider.GetComponent<PlayerInfo>().team.IsOpponnentOf(spInfos.team));
    }

    /// <summary>
    /// Renvoie vrai si il y a un defenseur entre l'IA et les cages adverses
    /// </summary>
    public bool IsDefenderReady() => IsDefenderReady(this.gameObject);

    /// <summary>
    /// Renvoie vrai si l'IA est demarquee
    /// </summary>
    public bool IsFree(GameObject ai, float threshold = 40f)
    {
        return Vector3.Distance(
                   GetNearestPlayer(
                   player => player.GetComponent<PlayerInfo>().team.IsOpponnentOf(ai.GetComponent<PlayerInfo>().team),
                   ai.transform.position
                   ).transform.position
                   ,
                   ai.transform.position
              ) < threshold;
    }

    /// <summary>
    /// Renvoie l'allie demarque le plus proche
    /// </summary>
    public GameObject GetNearestFreeAlly()
    {
        return GetNearestPlayer(
            player => player.GetComponent<PlayerInfo>().team == infos.team && IsFree(player),
            transform.position,
            true
        );
    }
    
    
}