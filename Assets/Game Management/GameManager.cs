using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private GameConfig gameConfig;   //Config de la partie (nombre max de buts, temps max etc)
    public int blueScore;            //Score de l'equipe bleu
    public int orangeScore;          //Score de l'equipe orange
    public float timeLeft;           //Temps restant a la partie en secondes
    public bool gamePlaying;         //Booleen indiquant que la partie est en cours et que le temps s'ecoule                                                                         // Donnees du jeu

    private Transform orangeSpawn;   //Position de spawn orange
    private Transform blueSpawn;     //Position de spawn bleu
    private Vector3 ballSpawn;       //Position de spawn de la balle
    private GameObject ball;         //Reference a la balle

    void Start()
    {
        orangeSpawn = GameObject.Find("orange_pos").transform;
        blueSpawn = GameObject.Find("blue_pos").transform;
        ballSpawn = Vector3.zero;
        ball = GameObject.FindGameObjectWithTag("Ball");
        
        gameConfig = GameConfig.Preset("Classic");
        
        timeLeft = gameConfig.gameDuration; // Definition de la duree de la partie
        blueScore = 0;
        orangeScore = 0;
        gamePlaying = false;
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
    }

    // Lancer une partie
    public void StartGame()                                                                
    {
        RespawnAll();
    }

    // Appelee des qu'il y a un but avec true si l'equipe bleue marque et false si l'equipe orange marque
    public void OnGoal(bool isForBlue)               
    {                                                  
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
        // Parcours les joueurs
        foreach(GameObject player in GameObject.FindGameObjectsWithTag("Player"))                      
        {
            if (player.GetComponent<PlayerInfo>().team == PlayerInfo.Team.Blue)                         // Si c'est un joueur de l'equipe bleue le respawn a son spawn
                player.transform.SetPositionAndRotation(blueSpawn.position, blueSpawn.rotation);
            else                                                                                        // Sinon le spawn a celui de l'equipe orange    
                player.transform.SetPositionAndRotation(orangeSpawn.position, orangeSpawn.rotation);
            
            //Si c'est le joueur local, on bloque ses inputs
            if (player.GetComponent<PhotonView>().IsMine && player.GetComponent<PlayerInfo>().isPlayer)
                player.GetComponent<InputManager>().StopInputs(4);
            //Si c'est une IA on lui dit de ne pas bouger
            if (!player.GetComponent<PlayerInfo>().isPlayer)
                player.GetComponent<Skills>().timeToMove = 4;
        }
        
        ball.transform.position = ballSpawn;
        ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
        gamePlaying = true;
    }

    private void EndGame()
    {
        Debug.Log("GameEnded");
    }
}
