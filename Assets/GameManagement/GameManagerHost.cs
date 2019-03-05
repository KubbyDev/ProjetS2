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
    
    //Met les joueurs qui n'ont pas de team dans des teams aleatoires (et equilibre)
    public static void SetTeams()
    {
        int blue = 0;
        int orange = 0;
        
        //On compte le nombre de joueurs deja presents dans chaque team
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (player.GetComponent<PlayerInfo>().team == Team.Blue)
                blue++;
            if (player.GetComponent<PlayerInfo>().team == Team.Orange)
                orange++;
        }
        
        //On place dans une team tous les joueurs qui ne sont pas encore dans une team
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            PlayerInfo infos = player.GetComponent<PlayerInfo>();
            
            //Si il est deja dans une team on passe
            if (infos.team != Team.None) 
                continue;

            //La nouvelle team est une team random si les deux teams ont autant de membres
            //Sinon c'est celle qui a le moins de membres
            Team newTeam = 
                blue == orange ? Teams.Random() 
                : blue > orange ? Team.Orange : Team.Blue;

            if (newTeam == Team.Blue)
            {
                infos.SetTeam(Team.Blue);
                blue++;
            }
            else
            {
                infos.SetTeam(Team.Orange);
                orange++;
            }
            
            //Informe tous les clients de la team choisie
            infos.UpdateInfo();
        }
    }
    
    //Rempli les places inutilisees dans la team avec des IA
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
            PlayerInfo newIaInfos = PhotonNetwork.Instantiate(Path.Combine("AI", "AI"), new Vector3(0, 10, 0), Quaternion.identity).GetComponent<PlayerInfo>();
            newIaInfos.GetComponent<PlayerInfo>().SetTeam(Team.Blue);
            newIaInfos.GetComponent<PlayerInfo>().UpdateInfo();
        }

        for (int i = orange; i < playersPerTeam; i++)
        {
            PlayerInfo newIaInfos = PhotonNetwork.Instantiate(Path.Combine("AI", "AI"), new Vector3(0, 10, 0), Quaternion.identity).GetComponent<PlayerInfo>();
            newIaInfos.GetComponent<PlayerInfo>().SetTeam(Team.Orange);
            newIaInfos.GetComponent<PlayerInfo>().UpdateInfo();
        }
    }
} 