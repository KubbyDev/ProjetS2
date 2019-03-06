using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PreGameManager : MonoBehaviour
{
    [SerializeField] private GameObject pregameMenu; //Le canvas qui contient les affichage de pregame
    
    public static float timeLeftToStart = 20;  //Temps avant le debut de la game
    public bool forceStart;                    //Permet de forcer le demarrage depuis l'inspector
    public static int maxPlayers;              //Le nombre max de joueurs dans la partie
    
    private bool initDone = false;
    private Text timeDisplayer;       //Le component qui affiche le texte pour le temps restant
    private Text playersDisplayer;    //Le component qui affiche le texte pour le nombre de joueurs

    void Start()
    {
        timeDisplayer = pregameMenu.transform.Find("Time").GetComponent<Text>();
        playersDisplayer = pregameMenu.transform.Find("Players").GetComponent<Text>();
    }
    
    void Update()
    {
        if (timeLeftToStart > 0)
            timeLeftToStart -= Time.deltaTime;
        
        //5 secondes avant le debut de la game
        if (timeLeftToStart < 5 && !initDone)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false; //On ferme la salle
            Ball.Hide();                              //On cache la balle
            
            //Uniquement sur le host
            if (PhotonNetwork.IsMasterClient)
            {
                GameManagerHost.SetTeams();     //On met tous les joueurs dans une team
                GameManagerHost.FillWithAIs();  //On met des IA a la place des joueurs manquants
            }
            
            initDone = true;
        }
        
        //Demarrage de la partie
        if (!GameManager.gameStarted && CanStartGame())
        {
            GameManager.script.StartGame();
            GameManager.gameStarted = true;
            pregameMenu.SetActive(false);
        }

        timeDisplayer.text = "The game starts in " + GameManager.FormatTime(timeLeftToStart);
        playersDisplayer.text = "Players: (" + PhotonNetwork.CurrentRoom.PlayerCount + "/" + maxPlayers + ")";
    }

    private bool CanStartGame()
    {
        return forceStart || timeLeftToStart < 0;
        //|| PhotonNetwork.CurrentRoom.PlayerCount >= GameManager.maxPlayers;
    }
}