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
    /// <returns></returns>
    public GameObject GetNearestPlayer(Func<GameObject, bool> filter, Vector3 fromPosition)
    {
        float minDist = 0;
        GameObject nearest = null;

        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player").Where(filter))
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
    /// Renvoie l'adversaire le plus proche des buts allies
    /// </summary>
    /// <returns></returns>
    public GameObject GetNearestOpponentFromGoal()
    {
        return GetNearestPlayer(
            player => player.GetComponent<PlayerInfo>().team.IsOpponnentOf(infos.team),
            GetAllyGoal().transform.position
        );  
    }
}