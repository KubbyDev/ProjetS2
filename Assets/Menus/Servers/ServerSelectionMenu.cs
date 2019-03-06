using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ServerSelectionMenu : MonoBehaviourPunCallbacks
{
    [SerializeField] private Canvas online;                              //Le menu avant la connection au serveur
    [SerializeField] private Canvas offline;                             //Le menu apres la connection au serveur
    [SerializeField] private InputField createRoomInput;                 //Le texte tappe dans le champ room name de create room
    [SerializeField] private Dropdown teamsSizeDropdown;                 //La taille des teams choisies (1v1, 2v2, 3v3, 4v4)
    [SerializeField] private InputField joinRoomInput;                   //Le texte tappe dans le champ room name de join room

    private ErrorMessage errorMessage;                                   //Le script qui gere l'affichage des messages d'erreur

    void Start()
    {
        errorMessage = online.GetComponentInChildren<ErrorMessage>(true);
    }

    //Quand la connection est etablie
    //Appellee par MainMenu.cs
    public void Connected()
    {
        //Met a jour l'affichage
        online.gameObject.SetActive(true);
        offline.gameObject.SetActive(false);
    }

    // Events des boutons ----------------------------------------------------------------------

    public void OnJoinRandomRoomClicked()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public void OnJoinRoomClicked()
    {
        if(joinRoomInput.text != "")
            //On rejoint la salle qui a le nom que le joueur a passe en parametre
            PhotonNetwork.JoinRoom(joinRoomInput.text);
        else
            errorMessage.Display("Please specify a room name");
    }

    public void OnCreateRoomClicked()
    {
        //Si le champ est rempli on donne a la salle le nom choisi, sinon on donne un nom random
        string text = createRoomInput.text;
        string name = text != "" ? text : "Room" + Random.Range(0, 1000000);

        CreateRoom(name, teamsSizeDropdown.value +1, int.MaxValue, 5*60);
    }

    // Connection aux salles ------------------------------------------------------------------

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        errorMessage.Display("No room found with that name");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        errorMessage.Display("No room found");
    }

    // Creation de salles ---------------------------------------------------------------------

    private static void CreateRoom(string name, int playersPerTeam, int maxGoals, int gameDurationInSec)
    {
        Hashtable gameConfig = new Hashtable
        {
            {"p", playersPerTeam}, 
            {"g", maxGoals},
            {"d", gameDurationInSec}
        };

        RoomOptions rops = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte) (playersPerTeam*2), CustomRoomProperties = gameConfig};
        PhotonNetwork.CreateRoom(name, rops);
    }

    public override void OnCreatedRoom()
    {
        //On charge le BasicField
        PhotonNetwork.LoadLevel(1);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorMessage.Display("Room creation failed");
    }
}
