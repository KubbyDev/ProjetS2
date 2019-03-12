using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PreGameManager : MonoBehaviour
{
    public static PreGameManager script;
    
    [SerializeField] private GameObject pregameMenu; //Le canvas qui contient les affichages de pregame
    
    public static float timeLeftToStart = 20;  //Temps avant le debut de la game
    public static int maxPlayers;              //Le nombre max de joueurs dans la partie

    private bool gameStarting;        //Passe a true a la fin du temps
    private Text timeDisplayer;       //Le component qui affiche le texte pour le temps restant
    private Text playersDisplayer;    //Le component qui affiche le texte pour le nombre de joueurs

    private void Awake()
    {
        script = this;
        timeLeftToStart = 20;
    }

    void Start()
    {
        timeDisplayer = pregameMenu.transform.Find("Time").GetComponent<Text>();
        playersDisplayer = pregameMenu.transform.Find("Players").GetComponent<Text>();
    }
    
    void Update()
    {
        //Si la game a deja demarre tout ca ne sert a rien
        if(gameStarting)
            return;
        
        if (timeLeftToStart > 0)
            timeLeftToStart -= Time.deltaTime;
        
        //Quand la salle est pleine on reduit le temps avant le lancement a 15 sec
        if (PhotonNetwork.CurrentRoom.PlayerCount >= maxPlayers && timeLeftToStart > 15)
            timeLeftToStart = 15;

        //On affiche le temps
        timeDisplayer.text = "The game starts in " + GameManager.FormatTime(timeLeftToStart);
        playersDisplayer.text = "Players: (" + PhotonNetwork.CurrentRoom.PlayerCount + "/" + maxPlayers + ")";
        
        //Quand le timer est fini, uniquement sur le host
        if (timeLeftToStart < 0 && PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false; //On ferme la salle

            GameManagerHost.SetTeams();     //On met tous les joueurs dans une team
            GameManagerHost.FillWithAIs();  //On met des IA a la place des joueurs manquants
            GameManagerHost.StartGame();    //Demarrage de la partie
        }
    }

    public void StartGame()
    {
        Ball.Hide();  //On cache la balle
        timeDisplayer.text = "The game is starting...";
        gameStarting = true;
        
        //timeLeftToStart est set par le GameManagerHost a 3 sec (- la latence)
        StartCoroutine(StartGame_Coroutine());
    }

    IEnumerator StartGame_Coroutine()
    {
        yield return new WaitForSeconds(timeLeftToStart);
        GameManager.script.StartGame();
        pregameMenu.SetActive(false);
    }
}