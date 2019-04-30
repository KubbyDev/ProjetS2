using System.IO;
using Photon.Pun;
using UnityEngine;

//Ce script donne les ordres au GameManager de tous les clients (et du host) pour gerer le deroulement de la partie

public class GameManagerHost : MonoBehaviourPunCallbacks
{
    public static int maxGoals;  //Le nombre de but pour que la partie se termine: 0 = infini
    public static bool inOvertime = false;
    
    void Start()
    {
        maxGoals = GameManager.gameConfig.maxGoals;
    }

    void Update()
    {
        //Seul le host execute ce code
        if (!PhotonNetwork.IsMasterClient)
            return;
        
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
    public static void OnGoal(bool isForBlue, Vector3 ballPosition)
    {
        //5 sec de celebration, 3 sec avant l'engagement
        GameManager.timeLeftForKickoff = 8;
     
        GameManager.script.OnGoal(isForBlue, ballPosition);
        
        GameDataSync.SendOnGoalEvent(isForBlue, ballPosition);

        //Si le nombre de buts max est depasse, la partie s'arrete
        if (maxGoals > 0 && (GameManager.blueScore >= maxGoals || GameManager.orangeScore >= maxGoals))
            EndGame();
        
        if(inOvertime)
            EndGame();
    }
    
    //Evenement de fin de partie
    //Cette methode va informer tous les clients que la partie est terminee
    private static void EndGame()
    {
        Team losingTeam = Team.None;
        if (GameManager.blueScore > GameManager.orangeScore)
            losingTeam = Team.Orange;
        if (GameManager.blueScore < GameManager.orangeScore)
            losingTeam = Team.Blue;
        
        //Appelle EndGame sur le GameManager de tous les clients
        GameDataSync.SendEndGameEvent(losingTeam);
        GameManager.script.EndGame(losingTeam);
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

        //On rempli les trous dans la team Bleu
        for (int i = blue; i < playersPerTeam; i++)
        {
            //Cree une IA et recupere son PlayerInfo
            PlayerInfo newIaInfos = PhotonNetwork.InstantiateSceneObject("AI/AI", new Vector3(0, 10, 0), Quaternion.identity).GetComponent<PlayerInfo>();
            
            //Change le hero de l'IA
            newIaInfos.SetHero(Heroes.Random());
            
            //Change la team de l'IA
            newIaInfos.SetTeam(Team.Blue);
            
            //Informe les autres clients de la team choisie
            newIaInfos.UpdateInfos();
        }

        //On rempli les trous dans la team Orange
        for (int i = orange; i < playersPerTeam; i++)
        {
            PlayerInfo newIaInfos = PhotonNetwork.InstantiateSceneObject("AI/AI", new Vector3(0, 10, 0), Quaternion.identity).GetComponent<PlayerInfo>();
            newIaInfos.SetHero(Heroes.Random());
            newIaInfos.SetTeam(Team.Orange);
            newIaInfos.UpdateInfos();
        }
    }
} 