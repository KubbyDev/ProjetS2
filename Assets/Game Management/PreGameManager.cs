using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PreGameManager : MonoBehaviour
{
    [SerializeField] private GameObject pregameMenu; //Le canvas qui contient les affichage de pregame
    
    public bool forceStart;           //Permet de forcer le demarrage depuis l'inspector

    private Text timeDisplayer;       //Le component qui affiche le texte pour le temps restant
    private Text playersDisplayer;    //Le component qui affiche le texte pour le nombre de joueurs
    private float timeLeft = 120;     //La partie demarre apres 120 secondes

    void Start()
    {
        timeDisplayer = pregameMenu.transform.Find("Time").GetComponent<Text>();
        playersDisplayer = pregameMenu.transform.Find("Players").GetComponent<Text>();
    }
    
    void Update()
    {
        if (timeLeft > 0)
            timeLeft -= Time.deltaTime;
        
        //5 secondes avant le debut de la game, on ferme la salle
        if (timeLeft < 5)
            PhotonNetwork.CurrentRoom.IsOpen = false;
        
        if (!GameManager.gameStarted && CanStartGame())
        {
            GameManager.script.StartGame();
            GameManager.gameStarted = true;
            pregameMenu.SetActive(false);
        }

        timeDisplayer.text = "The game starts in " + FormatTime(timeLeft);
        playersDisplayer.text = "Players: (" + PhotonNetwork.CurrentRoom.PlayerCount + "/" + GameManager.maxPlayers + ")";
    }

    private bool CanStartGame()
    {
        return forceStart || timeLeft < 0;
        //|| PhotonNetwork.CurrentRoom.PlayerCount >= GameManager.maxPlayers;
    }

    private string FormatTime(float time)
    {
        return ((int) (time+0.99f)/60).ToString().PadLeft(2, '0') + ":" + ((int) (time+0.99f)%60).ToString().PadLeft(2, '0');
    }
}