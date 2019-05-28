using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

//Ce script gere le deroulement de la partie (il recoit les ordres importants du GameManagerHost qui tourne sur le host)

public class GameManager : MonoBehaviour
{
    public static GameManager script;       //Reference a ce script, visible partout (accessible par GameManager.script)
    public static GameConfig gameConfig;    //Config de la partie (nombre max de buts, temps max etc)

    public static float timeLeft;           //Temps de jeu restant
    public static float timeLeftForKickoff; //Temps avant l'engagement. Ce temps est set par le GameDataSync, et prend en compte le temps de trajet de l'information
    public static bool gamePlaying;         //Booleen indiquant que la partie est en cours et que le temps s'ecoule
    
    public static int blueScore;             //Score de l'equipe bleu
    public static int orangeScore;           //Score de l'equipe orange
    public static bool gameStarted;          //Passe a true des que la partie demarre
    public static bool gameFinished;         //Passe a true des que la partie se termine
    
    [SerializeField] private Transform menus;                  //Contient les affichages
    [SerializeField] private Vector3 ballSpawn = Vector3.zero; //Position de spawn de la balle
    
    void Awake()
    {
        script = this;
        
        //Met a jour la configuration de la partie
        gameConfig = PhotonNetwork.CurrentRoom.CustomProperties.Config();
        PreGameManager.maxPlayers = 2 * gameConfig.playersPerTeam;
    }

    void Start()
    {
        Spawns.FindSpawns();           //Demande au script qui gere les spawns de trouver les spawns sur la scene
        
        blueScore = 0;
        orangeScore = 0;
        gamePlaying = false;
        gameStarted = false;
    }
    
    void Update()
    {
        if (gamePlaying) // Si la partie joue on enleve le temps ecoule au temps restant   
            timeLeft -= Time.deltaTime;

        if (timeLeftForKickoff > 0)
            timeLeftForKickoff -= Time.deltaTime;
        
        GameMenu.script.UpdateTimeDisplay(timeLeft);
    }
    
    // Debut de partie -------------------------------------------------------------------------------------------------
    
    // Lancer une partie
    //Pour que cette methode soit appelle, il faut que le Host declenche l'evenement de debut de partie
    //Puis que le GameDataSync envoie l'evenement a ce client dans PreGameManager.StartGame, puis que ca arrive a cette methode 
    public void StartGame()
    {
        gameStarted = true;
        gamePlaying = true;
        timeLeft = gameConfig.gameDuration;
        timeLeftForKickoff = 3;
        
        //Initialise le tableau des scores
        GameMenu.script.OnStartGame();

        //Cette methode remet tout le monde a sa place (AI, joueurs et balle)
        //Et demarre les temps avant de pouvoir bouger
        RespawnAll();
    }

    // Evenement de But ------------------------------------------------------------------------------------------------
    
    // Appelee des qu'il y a un but avec true si l'equipe bleue marque et false si l'equipe orange marque
    //Pour que cette methode soit appelle, il faut que le Host declenche l'evenement de but
    //Puis que le GameDataSync envoie l'evenement a ce client, et appelle cette methode
    public void OnGoal(bool isForBlue, Vector3 ballPosition)
    {
        //On fait une GoalExplosion dans le bon but
        GoalDetector.goals[isForBlue ? 1 : 0].GetComponent<GoalExplosion>().MakeGoalExplosion(ballPosition);
        
        //Si la partie n'a pas encore demarre on ne fait que l'explosion
        if (!gameStarted)
            return;
            
        //Incremente le nombre de buts marques du joueur qui a marque
        GameObject playerWhoScored = null;
        if (isForBlue)
        {
            if (Ball.script.shooterBlue != null)
                playerWhoScored = Ball.script.shooterBlue;
        }
        else
        {
            if(Ball.script.shooterOrange != null)
                playerWhoScored = Ball.script.shooterOrange;    
        }

        if (playerWhoScored != null)
            playerWhoScored.GetComponent<PlayerInfo>().goalsScored++;
        
        Ball.script.shooterBlue = Ball.script.shooterOrange = null;
        
        if (isForBlue)
            blueScore++;    // Ajoute un point aux bleus
        else
            orangeScore++;  // Ajoute un point aux oranges
        
        //Met a jour les points en haut de l'ecran
        GameMenu.script.OnScore(playerWhoScored, blueScore, orangeScore);
        
        //Si la partie est deja finie on ne fait que l'explosion et l'incrementation des points
        if (gameFinished)
            return;
        
        gamePlaying = false; //Empeche le temps de s'ecouler
        Ball.Hide();
        
        //Attend 5 secondes puis on lance le timer de l'engagement
        StartCoroutine(Celebration_Coroutine());
    }

    IEnumerator Celebration_Coroutine()
    {
        // Parcours les joueurs
        foreach(GameObject player in GameObject.FindGameObjectsWithTag("Player"))                      
        {
            //Si c'est une IA on lui dit de ne pas bouger jusqu'a l'engagement (8 secondes)
            if (! player.GetComponent<PlayerInfo>().isPlayer)
                player.GetComponent<Skills>().timeToMove = timeLeftForKickoff;
        }
        
        //Attend 5 secondes
        yield return new WaitForSeconds(timeLeftForKickoff - 3);
        
        //Cette methode remet tout le monde a sa place (AI, joueurs et balle)
        //Et demarre les temps avant de pouvoir bouger
        if(!gameFinished)
            RespawnAll();
    }
    
    // Engagement ------------------------------------------------------------------------------------------------------
    
    //Cette methode remet tout le monde a sa place (AI, joueurs et balle)
    //Et demarre les temps avant de pouvoir bouger
    public void RespawnAll()
    {
        gamePlaying = false; //Empeche le temps de s'ecouler
        Ball.Hide();
        
        //On fait respawn tout le monde
        Spawns.AssignSpawns(GameObject.FindGameObjectsWithTag("Player"));
        //Quand un spawn est utilise, il n'est plus utilisable apres, cette methode rend tous les spawns utilisables
        Spawns.ResetUsage(); 
        
        //On bloque les inputs du joueur local pendant 3 sec et reinitialise sa vitesse
        PlayerInfo.localPlayer.GetComponent<InputManager>().StopInputs(3);
        PlayerInfo.localPlayer.GetComponent<MovementManager>().ResetSpeed();
        
        //On reset tous les enregistrement des derniers inputs
        //Sans ca, les avatars des autres joueurs pourraient se mettre a bouger au debut des 3 secondes
        // + Reset des cooldowns
        // + Blocage des IA
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            PlayerInfo infos = player.GetComponent<PlayerInfo>();
            infos.lastMovementInput = Vector3.zero;
            infos.ResetCooldowns();
            infos.StopSpells();
            
            //Si c'est une IA on lui dit de ne pas bouger jusqu'a l'engagement (3 secondes)
            if (! player.GetComponent<PlayerInfo>().isPlayer)
                player.GetComponent<Skills>().timeToMove = timeLeftForKickoff;
        }
        
        //On kill tous les elements lances par des spells
        foreach (GameObject spellItem in GameObject.FindGameObjectsWithTag("Spell"))
        {
            Destroy(spellItem);
            spellItem.SetActive(false);
        }

        //On attend 3 secondes puis on relance la game
        GameMenu.script.DisplayKickoffCountdown();
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
    
    //Pour que cette methode soit appelle, il faut que le Host declenche l'evenement de fin de partie
    //Puis que le GameDataSync envoie l'evenement a ce client, et appelle cette methode
    public static void EndGame(Team losingTeam)
    {
        gamePlaying = false;
        gameFinished = true;
        Ball.Hide();
        
        GameMenu.script.OnWin(losingTeam.OtherTeam());
        EndGameManager.script.EndGame(losingTeam);
    }
}