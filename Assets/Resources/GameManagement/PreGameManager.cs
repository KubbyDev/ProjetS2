using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

//Ce script gere la pregame
//

public class PreGameManager : MonoBehaviour
{
    public static PreGameManager script;

    [SerializeField] private float pregameDuration = 120;  //La duree maximale de la pregame en secondes
    [SerializeField] private GameObject pregameMenu;       //Le canvas qui contient les affichages de pregame
    
    public static float timeLeftToStart;       //Temps avant le debut de la game
    public static int maxPlayers;              //Le nombre max de joueurs dans la partie

    public bool gameStarting;         //Passe a true a la fin du temps
    private Text timeDisplayer;       //Le component qui affiche le texte pour le temps restant
    private Text playersDisplayer;    //Le component qui affiche le texte pour le nombre de joueurs

    private void Awake()
    {
        script = this;
        timeLeftToStart = pregameDuration;
    }

    void Start()
    {
        //Recupere les afficheurs pour le nombre de joueurs max et le temps avant que la partie demarre
        timeDisplayer = pregameMenu.transform.Find("Time").GetComponent<Text>();
        playersDisplayer = pregameMenu.transform.Find("Players").GetComponent<Text>();
    }
    
    void Update()
    {
        //Si la game a deja demarre ou que le client a plante tout ca ne sert a rien
        if(gameStarting || PhotonNetwork.CurrentRoom == null)
            return;
        
        //On ferme la salle si elle est pleine
        PhotonNetwork.CurrentRoom.IsOpen = PhotonNetwork.CurrentRoom.PlayerCount < maxPlayers;
        
        if (timeLeftToStart > 0)
            timeLeftToStart -= Time.deltaTime;
        
        //Quand la salle est pleine on reduit le temps avant le lancement a 15 sec
        if (PhotonNetwork.CurrentRoom.PlayerCount >= maxPlayers && timeLeftToStart > 15)
            timeLeftToStart = 15;

        //On affiche le temps
        timeDisplayer.text = "The game starts in " + Tools.FormatTime(timeLeftToStart);
        playersDisplayer.text = "Players: (" + PhotonNetwork.CurrentRoom.PlayerCount + "/" + maxPlayers + ")";
        
        //Quand le timer est fini, uniquement sur le host
        if (timeLeftToStart < 0 && PhotonNetwork.IsMasterClient)
            StartCoroutine(HandleGameStart_Coroutine());
    }

    //Executee sur le host au moment du demarrage de la partie
    IEnumerator HandleGameStart_Coroutine()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false; //On ferme la salle
        
        Ball.Hide();  //On cache la balle
        timeDisplayer.text = "The game is starting...";
        gameStarting = true;
        
        yield return new WaitForSeconds(2);
        
        GameManagerHost.SetTeams();     //On met tous les joueurs dans une team
        GameManagerHost.FillWithAIs();  //On met des IA a la place des joueurs manquants

        GameDataSync.SendStartGameEvent();
        timeLeftToStart = 2;
        StartGame();
    }
    
    //Pour que cette methode soit appelle, il faut que le Host declenche l'evenement de debut de partie
    //Puis que le GameDataSync envoie l'evenement a ce client, et appelle cette methode
    public void StartGame()
    {
        Ball.Hide();  //On cache la balle
        timeDisplayer.text = "The game is starting...";
        gameStarting = true;
        
        //Affiche le temps en haut de l'ecran
        GameObject.Find("Menus").transform.Find("GameMenu").gameObject.SetActive(true);
        
        //Separe les teams dans le menu tab
        GameObject.Find("Menus").transform.Find("Tab").GetComponent<TabMenu>().SeparateTeams();
        
        //timeLeftToStart est set par le GameManagerHost a 3 sec (- la latence)
        StartCoroutine(StartGame_Coroutine());
    }

    //Executee sur tous les joueurs
    IEnumerator StartGame_Coroutine()
    {
        yield return new WaitForSeconds(timeLeftToStart);
        GameManager.script.StartGame();
        pregameMenu.SetActive(false);
    }
}