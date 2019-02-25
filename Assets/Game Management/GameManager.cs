using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager script; //Reference a ce script, visible partout
    
    public static bool gamePlaying;   //Booleen indiquant que la partie est en cours et que le temps s'ecoule
    public static int maxPlayers;     //Le nombre de joueurs max en jeu
    public int blueScore;             //Score de l'equipe bleu
    public int orangeScore;           //Score de l'equipe orange
    public float timeLeft;            //Temps restant a la partie en secondes

    private bool gameStarted = false; //Passe a true des que la partie demarre
    private GameConfig gameConfig;    //Config de la partie (nombre max de buts, temps max etc)
    private Vector3 ballSpawn;        //Position de spawn de la balle

    private void Awake()
    {
        script = this;
        
        gameConfig = GameConfig.Preset("Classic");
        maxPlayers = gameConfig.playersPerTeam * 2;
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

    // Lancer une partie
    public void StartGame()
    {
        gameStarted = true;
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
        
        //Met a jour les timers des spawns. Quand un spawn est utilise il met 5 secondes a etre utilisable
        Spawns.UpdateTimers();
    }

    // Appelee des qu'il y a un but avec true si l'equipe bleue marque et false si l'equipe orange marque
    public void OnGoal(bool isForBlue)
    {
        //Si la partie n'a pas encore demarre on ne fait rien
        if (!gameStarted)
            return;
            
        //Incremente le nombre de buts marques du joueur qui a marque
        Ball.script.shooter.GetComponent<PlayerInfo>().goalsScored++;
        
        if (isForBlue)
            blueScore++;    // Ajoute un point aux bleus
        else
            orangeScore++;  // Ajoute un point aux oranges
        
        gamePlaying = false;
        
        if(blueScore >= gameConfig.maxGoals || orangeScore >= gameConfig.maxGoals)
            EndGame();
        else
            RespawnAll();
    }

    // Reset la position des joueurs et de la balle
    private void RespawnAll()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        
        //On fait respawn tout le monde
        Spawns.AssignSpawns(players);

        // Parcours les joueurs
        foreach(GameObject player in players)                      
        {
            //Si c'est une IA on lui dit de ne pas bouger
            if (!player.GetComponent<PlayerInfo>().isPlayer)
                player.GetComponent<Skills>().timeToMove = 4;
        }
        
        //On bloque les inputs du joueur local pendant 4 sec
        PlayerInfo.localPlayer.GetComponent<InputManager>().StopInputs(4);
        PlayerInfo.localPlayer.GetComponent<MovementManager>().StopAllMovements();
        
        //On respawn la balle
        Ball.ball.transform.position = ballSpawn;
        Ball.script.StopAllMovements();
        Ball.script.UpdatePossessor(null);
        
        gamePlaying = true;
    }

    private void EndGame()
    {
        Debug.Log("GameEnded");
    }
}
