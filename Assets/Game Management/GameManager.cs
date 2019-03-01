using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager script;       //Reference a ce script, visible partout
    public static GameConfig gameConfig;    //Config de la partie (nombre max de buts, temps max etc)

    public static float timeLeft;           //Temps de jeu restant
    public static float timeLeftForKickoff; //Temps avant l'engagement  
    public static bool gamePlaying;   //Booleen indiquant que la partie est en cours et que le temps s'ecoule
    
    public static int blueScore;      //Score de l'equipe bleu
    public static int orangeScore;    //Score de l'equipe orange
    public static bool gameStarted;   //Passe a true des que la partie demarre
    public static int maxPlayers;     //Le nombre de joueurs max en jeu
    
    [SerializeField] private GameObject gameMenu; //Contient les affichages
    
    private Vector3 ballSpawn;           //Position de spawn de la balle
    private Text timeDisplayer;          //Le component qui affiche le temps restant
    private Text blueScoreDisplayer;     //Le component qui affiche le score de l'equipe bleu
    private Text orangeScoreDisplayer;   //Le component qui affiche le score de l'equipe orange
    
    void Awake()
    {
        script = this;
        
        timeDisplayer = gameMenu.transform.Find("Background").Find("Time").GetComponent<Text>();
        blueScoreDisplayer = gameMenu.transform.Find("Background").Find("BlueScore").GetComponent<Text>();
        orangeScoreDisplayer = gameMenu.transform.Find("Background").Find("OrangeScore").GetComponent<Text>();
    }

    void Start()
    {
        Spawns.FindSpawns();                //Demande au script qui gere les spawns de trouver les spawns sur la scene
        ballSpawn = Vector3.zero;
        
        timeLeft = gameConfig.gameDuration; // Definition de la duree de la partie
        blueScore = 0;
        orangeScore = 0;
        gamePlaying = false;
    }

    public void OnFirstPacketRecieved()
    {
        maxPlayers = 2 * (int) gameConfig.parameters[(int) GameConfig.Parameters.MaxGoals];
        
    }
    

    // Lancer une partie
    public void StartGame()
    {
        gameMenu.SetActive(true);
        
        // Parcours les joueurs
        foreach(GameObject player in GameObject.FindGameObjectsWithTag("Player"))                      
        {
            //Si c'est une IA on lui dit de ne pas bouger jusqu'a l'engagement
            if (!player.GetComponent<PlayerInfo>().isPlayer)
                player.GetComponent<Skills>().timeToMove = 3;
        }
        
        RespawnAll();
    }
    
    void Update()
    {
        if (gamePlaying)  // Si la partie joue on enleve le temps ecoule au temps restant
            timeLeft -= Time.deltaTime;
        
        if (timeLeft < 0) // Si le temps est ecoule, la partie s'arrete
        {
            timeLeft = 0;
            gamePlaying = false;
            EndGame();
        }
        
        //Met a jour le temps en haut de l'ecran
        timeDisplayer.text = FormatTime(timeLeft);
        
        //Met a jour les timers des spawns. Quand un spawn est utilise il met 5 secondes a etre utilisable
        Spawns.UpdateTimers();
    }

    // Appelee des qu'il y a un but avec true si l'equipe bleue marque et false si l'equipe orange marque
    public void OnGoal(bool isForBlue, Vector3 ballPosition)
    {
        //TODO: Deduire le but touche et appeller la goal explosion
        
        //Si la partie n'a pas encore demarre on ne fait rien
        if (!gameStarted)
            return;
            
        //Incremente le nombre de buts marques du joueur qui a marque
        Ball.script.shooter.GetComponent<PlayerInfo>().goalsScored++;
        
        if (isForBlue)
            blueScore++;    // Ajoute un point aux bleus
        else
            orangeScore++;  // Ajoute un point aux oranges
        
        //Met a jour les points en haut de l'ecran
        blueScoreDisplayer.text = blueScore.ToString();
        orangeScoreDisplayer.text = orangeScore.ToString();
        
        gamePlaying = false;
        
        if (blueScore >= gameConfig.maxGoals || orangeScore >= gameConfig.maxGoals)
            //Si la game est finie on l'arrete
            EndGame();
        else
            //Sinon on attend 5 secondes puis on lance le timer de l'engagement
            StartCoroutine(Celebration_Coroutine());
        
        //On cache la balle
        Ball.ball.transform.position = ballSpawn - new Vector3(0, -200, 0);
        Ball.script.StopAllMovements();
        Ball.script.UpdatePossessor(null);
        Ball.rigidBody.useGravity = false;
    }

    IEnumerator Celebration_Coroutine()
    {
        // Parcours les joueurs
        foreach(GameObject player in GameObject.FindGameObjectsWithTag("Player"))                      
        {
            //Si c'est une IA on lui dit de ne pas bouger jusqu'a l'engagement
            if (!player.GetComponent<PlayerInfo>().isPlayer)
                player.GetComponent<Skills>().timeToMove = 8;
        }
        
        yield return new WaitForSeconds(5);
        RespawnAll();
    }

    // Reset la position des joueurs et de la balle
    private void RespawnAll()
    {
        //On fait respawn tout le monde
        Spawns.AssignSpawns(GameObject.FindGameObjectsWithTag("Player"));
        
        //On bloque les inputs du joueur local pendant 3 sec
        PlayerInfo.localPlayer.GetComponent<InputManager>().StopInputs(3);
        PlayerInfo.localPlayer.GetComponent<MovementManager>().StopAllMovements();

        //On respawn la balle
        Ball.ball.transform.position = ballSpawn;
        Ball.script.StopAllMovements();
        Ball.script.UpdatePossessor(null);
        Ball.rigidBody.useGravity = true;
        
        //On attend 3 secondes puis on relance la game
        StartCoroutine(Kickoff_Coroutine());
    }

    IEnumerator Kickoff_Coroutine()
    {
        yield return new WaitForSeconds(3);
        gamePlaying = true;
    }

    private void EndGame()
    {
        Debug.Log("GameEnded");
    }
    
    private string FormatTime(float time)
    {
        return ((int) (time+0.99f)/60).ToString().PadLeft(2, '0') + ":" + ((int) (time+0.99f)%60).ToString().PadLeft(2, '0');
    }
}