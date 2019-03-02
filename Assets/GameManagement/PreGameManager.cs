using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PreGameManager : MonoBehaviour
{
    [SerializeField] private GameObject pregameMenu; //Le canvas qui contient les affichage de pregame
    
    public static float timeLeftToStart = 120; //Temps avant le debut de la game
    public bool forceStart;                    //Permet de forcer le demarrage depuis l'inspector
    public static int maxPlayers;              //Le nombre max de joueurs dans la partie
    
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
        
        //5 secondes avant le debut de la game, on ferme la salle
        if (timeLeftToStart < 5)
            PhotonNetwork.CurrentRoom.IsOpen = false;
        
        if (!GameManager.gameStarted && CanStartGame())
        {
            GameManager.script.StartGame();
            GameManager.gameStarted = true;
            pregameMenu.SetActive(false);
        }

        timeDisplayer.text = "The game starts in " + FormatTime(timeLeftToStart);
        playersDisplayer.text = "Players: (" + PhotonNetwork.CurrentRoom.PlayerCount + "/" + maxPlayers + ")";
    }

    private bool CanStartGame()
    {
        return forceStart || timeLeftToStart < 0;
        //|| PhotonNetwork.CurrentRoom.PlayerCount >= GameManager.maxPlayers;
    }

    private string FormatTime(float time)
    {
        return ((int) (time+0.99f)/60).ToString().PadLeft(2, '0') + ":" + ((int) (time+0.99f)%60).ToString().PadLeft(2, '0');
    }
}