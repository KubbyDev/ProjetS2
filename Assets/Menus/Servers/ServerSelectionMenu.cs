using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class ServerSelectionMenu : MonoBehaviourPunCallbacks
{
    [SerializeField] [Range(1, 100)] private byte maxPlayers = 10;       //Le nombre de joueurs que peuvent contenir les rooms creees par le bouton quick play (temporaire)
    [SerializeField] private Canvas online;                              //Le menu avant la connection au serveur
    [SerializeField] private Canvas offline;                             //Le menu apres la connection au serveur
    [SerializeField] private InputField CreateRoomInput;                 //Le texte tappe dans le champ room name de create room
    [SerializeField] private InputField JoinRoomInput;                   //Le texte tappe dans le champ room name de join room

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
        if(JoinRoomInput.text != "")
            //On rejoint la salle qui a le nom que le joueur a passe en parametre
            PhotonNetwork.JoinRoom(JoinRoomInput.text);
        else
            errorMessage.Display("Please specify a room name");
    }

    public void OnCreateRoomClicked()
    {
        //Si le champ est rempli on donne a la salle le nom choisi, sinon on donne un nom random
        string name = (CreateRoomInput.text != "") ? CreateRoomInput.text : "Room" + (int)Random.Range(0, 1000000);

        CreateRoom(name);
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

    private void CreateRoom(string name)
    {
        RoomOptions rops = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = maxPlayers };
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
