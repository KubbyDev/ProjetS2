using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

//Ce script gere le deroulement de la partie (il recoit les ordres importants du GameManagerHost qui tourne sur le host)

public class GameManager : MonoBehaviour
{
    public static GameManager script;       //Reference a ce script, visible partout
    public static GameConfig gameConfig;    //Config de la partie (nombre max de buts, temps max etc)

    public static float timeLeft;           //Temps de jeu restant
    public static float timeLeftForKickoff; //Temps avant l'engagement  
    public static bool gamePlaying;         //Booleen indiquant que la partie est en cours et que le temps s'ecoule
    
    public static int blueScore;      //Score de l'equipe bleu
    public static int orangeScore;    //Score de l'equipe orange
    public static bool gameStarted;   //Passe a true des que la partie demarre
    
    [SerializeField] private GameObject gameMenu; //Contient les affichages
    
    private Vector3 ballSpawn;           //Position de spawn de la balle
    private Text timeDisplayer;          //Le component qui affiche le temps restant
    private Text blueScoreDisplayer;     //Le component qui affiche le score de l'equipe bleu
    private Text orangeScoreDisplayer;   //Le component qui affiche le score de l'equipe orange
    
    void Awake()
    {
        script = this;
        
        //Met a jour la configuration de la partie
        gameConfig = PhotonNetwork.CurrentRoom.CustomProperties.Config();
        PreGameManager.maxPlayers = 2 * gameConfig.playersPerTeam;
        
        timeDisplayer = gameMenu.transform.Find("Background").Find("Time").GetComponent<Text>();
        blueScoreDisplayer = gameMenu.transform.Find("Background").Find("BlueScore").GetComponent<Text>();
        orangeScoreDisplayer = gameMenu.transform.Find("Background").Find("OrangeScore").GetComponent<Text>();
    }

    void Start()
    {
        Spawns.FindSpawns();                //Demande au script qui gere les spawns de trouver les spawns sur la scene
        ballSpawn = Vector3.zero;
        
        blueScore = 0;
        orangeScore = 0;
        gamePlaying = false;
        gameStarted = false;
    }
    
    void Update()
    {
        if (gamePlaying)  // Si la partie joue on enleve le temps ecoule au temps restant
            timeLeft -= Time.deltaTime;
        
        //Met a jour le temps en haut de l'ecran
        timeDisplayer.text = FormatTime(timeLeft);
    }
    
    //Met le temps en format MinMin:SecSec
    public static string FormatTime(float time)
    {
        if (time < 0)
            return "00:00";
        
        return ((int) (time+0.99f)/60).ToString().PadLeft(2, '0') + ":" + ((int) (time+0.99f)%60).ToString().PadLeft(2, '0');
    }
    
    // Debut de partie -------------------------------------------------------------------------------------------------
    
    // Lancer une partie
    public void StartGame()
    {
        //Affiche le temps en haut de l'ecran
        gameMenu.SetActive(true);
        
        gameStarted = true;
        timeLeft = gameConfig.gameDuration;
        
        // Parcours les joueurs
        foreach(GameObject player in GameObject.FindGameObjectsWithTag("Player"))                      
        {
            //Si c'est une IA on lui dit de ne pas bouger jusqu'a l'engagement
            if (! player.GetComponent<PlayerInfo>().isPlayer)
                player.GetComponent<Skills>().timeToMove = 3;
        }
        
        RespawnAll();
    }

    // Evenement de But ------------------------------------------------------------------------------------------------
    
    // Appelee des qu'il y a un but avec true si l'equipe bleue marque et false si l'equipe orange marque
    public void OnGoal(bool isForBlue, Vector3 ballPosition)
    {
        //On fait une GoalExplosion dans le bon but
        GoalDetector.goals[isForBlue ? 1 : 0].GetComponent<GoalExplosion>().MakeGoalExplosion(ballPosition);
        
        //Si la partie n'a pas encore demarre on ne fait rien
        if (!gameStarted)
            return;
            
        //Incremente le nombre de buts marques du joueur qui a marque
        if(Ball.script.shooter != null)
            Ball.script.shooter.GetComponent<PlayerInfo>().goalsScored++;
        
        if (isForBlue)
            blueScore++;    // Ajoute un point aux bleus
        else
            orangeScore++;  // Ajoute un point aux oranges
        
        //Met a jour les points en haut de l'ecran
        blueScoreDisplayer.text = blueScore.ToString();
        orangeScoreDisplayer.text = orangeScore.ToString();
        
        gamePlaying = false;
        Ball.Hide();
        
        //Attend 5 secondes puis on lance le timer de l'engagement
        StartCoroutine(Celebration_Coroutine());
    }

    IEnumerator Celebration_Coroutine()
    {
        // Parcours les joueurs
        foreach(GameObject player in GameObject.FindGameObjectsWithTag("Player"))                      
        {
            //Si c'est une IA on lui dit de ne pas bouger jusqu'a l'engagement
            if (! player.GetComponent<PlayerInfo>().isPlayer)
                player.GetComponent<Skills>().timeToMove = timeLeftForKickoff;
        }
        
        //Attend 5 secondes
        yield return new WaitForSeconds(timeLeftForKickoff - 3);
        
        RespawnAll();
    }
    
    // Engagement ------------------------------------------------------------------------------------------------------
    
    // Reset la position des joueurs et de la balle
    private void RespawnAll()
    {
        //On fait respawn tout le monde
        Spawns.AssignSpawns(GameObject.FindGameObjectsWithTag("Player"));
        Spawns.ResetUsage();
        
        //On bloque les inputs du joueur local pendant 3 sec
        PlayerInfo.localPlayer.GetComponent<InputManager>().StopInputs(3);
        PlayerInfo.localPlayer.GetComponent<MovementManager>().ResetSpeed();
        
        //On reset tous les enregistrement des derniers inputs
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
            player.GetComponent<PlayerInfo>().lastMovementInput = Vector3.zero;
        
        //On attend 3 secondes puis on relance la game
        StartCoroutine(Kickoff_Coroutine());
    }

    IEnumerator Kickoff_Coroutine()
    {
        yield return new WaitForSeconds(1);
        Ball.Respawn(ballSpawn);
        yield return new WaitForSeconds(2);
        gamePlaying = true;
    }

    // Fin de partie ---------------------------------------------------------------------------------------------------
    
    public static void EndGame()
    {
        Debug.Log("Game End");
    }
}