using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagement : MonoBehaviour
{
    float gameDuration;                                                                                 // Temps restant a la partie
    bool gamePlaying;                                                                                   // Booleen indiquant que la partie est en cours et que le temps s'ecoule
    private GameData gameData;                                                                          // Donnees du jeu

    public Transform orangeSpawn;
    public Transform blueSpawn;
    public Vector3 ballSpawn;


    private void Start()
    {
        orangeSpawn = GameObject.Find("orange_pos").transform;
        blueSpawn = GameObject.Find("blue_pos").transform;
        ballSpawn = Vector3.zero;

    }

    public void Start_game(int duration)                                                                // Lancer une partie en instanciant gameData en indiquant la duree de la partie
    {
        gameData = new GameData();                                                                      // Scores a 0
        gameDuration = duration;                                                                        // Definition de la duree de la partie
    }

    public void OnGoal(bool isBlue)                                                                     // A appeler des qu'il y a un but avec true si l'equipe bleue marque et false si l'equipe orange marque
    {                                                                                                   // Oe ca sert a rien mais c'est plus clair sorry 
        if (isBlue == true)
           gameData.BlueTeamScores();                                                                   // Appelle la fonction qui ajoute un point aux bleus
        else
            gameData.OrangeTeamScores();                                                                // Appelle la fonction qui ajoute un point aux oranges
        gamePlaying = false;
    }

    public void TimeLeft()                                                                              // Met a jour le temps de partie restant A APPELER DANS LE GAME UPDATE
    {
        if (gamePlaying)                                                                                // Si la partie joue on enleve le temps ecoule au temps restant
            gameDuration -= Time.deltaTime;
        if (gameDuration <= 0)                                                                          // Si le temps est ecoule, la partie ne joue plus
        {
            gamePlaying = false;
            EndGame();
        }

    }

    public void NewPoint()                                                                              // Nouveau point, reset la position des joueurs
    {
        foreach(GameObject player in GameObject.FindGameObjectsWithTag("Player"))                       // Parcours les joueurs
        {
            if (player.GetComponent<PlayerInfo>().team == PlayerInfo.Team.Blue)                         // Si c'est un joueur de l'equipe bleue le respawn a son spawn
                player.transform.SetPositionAndRotation(blueSpawn.position, blueSpawn.rotation);
            else                                                                                        // Sinon le spawn a celui de l'equipe orange    
                player.transform.SetPositionAndRotation(orangeSpawn.position, orangeSpawn.rotation);

        }

        GameObject.FindGameObjectWithTag("Ball").transform.position = ballSpawn;
        StartCoroutine(NewPointDelay());
        gamePlaying = true;
    }

    IEnumerator NewPointDelay()                                                                         // Empeche les joueurs de se deplacer directement apres le respawn
    {
        GetComponent<InputManager>().enabled = false;                                                   // Stop les input
        yield return new WaitForSeconds(4f);                                                            // Attend
        GetComponent<InputManager>().enabled = true;                                                    // Enable les inputs
    }

    public void EndGame()
    {

    }
}
