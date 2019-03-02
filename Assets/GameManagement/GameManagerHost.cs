using System.Net;
using Photon.Pun;
using UnityEngine;

public class GameManagerHost : MonoBehaviour
{
    public static int maxGoals;
    
    void Awake()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            this.enabled = false;
            return;
        }
        
        GameManager.gameConfig = GamePreset.Classic.Config();
        maxGoals = (int) GameManager.gameConfig.parameters[(int) GameConfig.Parameters.MaxGoals];
        PreGameManager.maxPlayers = 2 * (int) GameManager.gameConfig.parameters[(int) GameConfig.Parameters.PlayersPerTeam];
    }

    void Update()
    {
        if (GameManager.timeLeft < 0) // Si le temps est ecoule, la partie s'arrete
        {
            GameManager.timeLeft = 0;
            GameManager.gamePlaying = false;
            EndGame();
        }
    }

    //Le host recoit l'event de but et informe tous les clients
    public static void OnGoal(bool isBlue)
    {
        //5 sec de celebration, 3 sec avant l'engagement
        GameManager.timeLeftForKickoff = 8;
        
        GameDataSync.SendOnGoalData(isBlue);

        if (GameManager.blueScore >= maxGoals || GameManager.orangeScore >= maxGoals)
            EndGame();
    }

    private static void EndGame()
    {
        
    }
} 