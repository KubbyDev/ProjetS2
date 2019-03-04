using System.IO;
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
        //Appelle EndGame sur le GameManager de tous les clients
        GameDataSync.SendEndGameEvent();
        GameManager.EndGame();
    }

    public static void FillWithAIs()
    {
        int playersPerTeam = (int) GameManager.gameConfig.parameters[(int) GameConfig.Parameters.PlayersPerTeam];
        
        //On set la team du joueur en fonction des nombres de joueurs dans les autres teams
        int blue = 0;
        int orange = 0;
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
            if (player.GetComponent<PlayerInfo>().team == Team.Blue)
                blue++;
            else
                orange++;

        for (int i = blue; i < playersPerTeam; i++)
        {
            GameObject newIA = PhotonNetwork.Instantiate(Path.Combine("AI", "AI"), new Vector3(0, 10, 0), Quaternion.identity);
            newIA.GetComponent<PlayerInfo>().SetTeam(Team.Blue);
        }

        for (int i = orange; i < playersPerTeam; i++)
        {
            GameObject newIA = PhotonNetwork.Instantiate(Path.Combine("AI", "AI"), new Vector3(0, 10, 0), Quaternion.identity);
            newIA.GetComponent<PlayerInfo>().SetTeam(Team.Orange);
        }
    }
} 