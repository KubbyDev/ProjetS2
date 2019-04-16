using System.IO;
using Photon.Pun;
using UnityEngine;

//Ce script donne les ordres au GameManager de tous les clients (et du host) pour gerer le deroulement de la partie

public class GameManagerHost : MonoBehaviourPunCallbacks
{
    public static int maxGoals;  //Le nombre de but pour que la partie se termine: 0 = infini
    public static bool inOvertime = false;
    
    void Awake()
    {
        //Si on est pas le host
        if (!PhotonNetwork.IsMasterClient)
        {
            //On desactive ce script (plus aucune methode ne sera appellee ici)
            this.enabled = false;
            return;
        }
        
        //Uniquement sur le host
        
        //Met a jour la configuration de la partie
        //Recupere les CustomProperties (enregistrees dans la PhotonRoom a la creation de la salle)
        GameManager.gameConfig = PhotonNetwork.CurrentRoom.CustomProperties.Config();
        
        PreGameManager.maxPlayers = 2 * GameManager.gameConfig.playersPerTeam;
        maxGoals = GameManager.gameConfig.maxGoals;
    }

    void Update()
    {
        if (GameManager.timeLeft < 0 && !inOvertime) // Si le temps est ecoule, soit on lance l'overtime, soit on termine la partie
        {
            inOvertime = true;
            GameManager.timeLeft = 0;

            if (GameManager.blueScore == GameManager.orangeScore)
            {
                Ball.Hide();
                GameManager.timeLeftForKickoff = 3;
                GameDataSync.SendOvertimeEvent();
                GameManager.script.RespawnAll();
            }
            else
                EndGame();
        }
    }

    //Evenement de debut de partie
    //Cette methode va informer tous les clients que la partie demarre
    public static void StartGame()
    {
        GameDataSync.SendStartGameEvent();
        PreGameManager.timeLeftToStart = 3;
        PreGameManager.script.StartGame();
    }
    
    //Evenement de but
    //Cette methode va informer tous les clients qu'il y a un but
    public static void OnGoal(bool isForBlue)
    {
        if(inOvertime)
            EndGame();
        
        //5 sec de celebration, 3 sec avant l'engagement
        GameManager.timeLeftForKickoff = 8;
        
        GameDataSync.SendOnGoalEvent(isForBlue);

        //Si le nombre de buts max est depasse, la partie s'arrete
        if (maxGoals > 0 && (GameManager.blueScore >= maxGoals || GameManager.orangeScore >= maxGoals))
            EndGame();
    }
    
    //Evenement de fin de partie
    //Cette methode va informer tous les clients que la partie est terminee
    private static void EndGame()
    {
        //Appelle EndGame sur le GameManager de tous les clients
        GameDataSync.SendEndGameEvent(GameManager.blueScore > GameManager.orangeScore);
        GameManager.script.EndGame(GameManager.blueScore > GameManager.orangeScore);
    }

    // Debut de partie -------------------------------------------------------------------------------------------------
    
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
            infos.UpdateInfos();
        }
    }
    
    //Rempli les places inutilisees dans la team avec des IA
    public static void FillWithAIs()
    {
        int playersPerTeam = GameManager.gameConfig.playersPerTeam;
        
        //On set la team du joueur en fonction des nombres de joueurs dans les autres teams
        int blue = 0;
        int orange = 0;
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
            if (player.GetComponent<PlayerInfo>().team == Team.Blue)
                blue++;
            else
                orange++;

        //On rempli les trous dans chaque team
        for (int i = blue; i < playersPerTeam; i++)
        {
            //Cree une IA et recupere son PlayerInfo
            PlayerInfo newIaInfos = PhotonNetwork.Instantiate(Path.Combine("AI", "AI"), new Vector3(0, 10, 0), Quaternion.identity).GetComponent<PlayerInfo>();
            
            //Change la team de l'IA
            newIaInfos.GetComponent<PlayerInfo>().SetTeam(Team.Blue);
            
            //Informe les autres clients de la team choisie
            newIaInfos.GetComponent<PlayerInfo>().UpdateInfos();
        }

        //On rempli les trous dans la team Orange
        for (int i = orange; i < playersPerTeam; i++)
        {
            PlayerInfo newIaInfos = PhotonNetwork.Instantiate(Path.Combine("AI", "AI"), new Vector3(0, 10, 0), Quaternion.identity).GetComponent<PlayerInfo>();
            newIaInfos.GetComponent<PlayerInfo>().SetTeam(Team.Orange);
            newIaInfos.GetComponent<PlayerInfo>().UpdateInfos();
        }
    }
} 