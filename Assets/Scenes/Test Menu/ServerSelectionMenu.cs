using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ServerSelectionMenu : MonoBehaviourPunCallbacks
{
    [SerializeField] [Range(1, 100)] private byte maxPlayers = 10;       //Le nombre de joueurs que peuvent contenir les rooms creees par le bouton quick play (temporaire)
    [SerializeField] private Canvas online;
    [SerializeField] private Canvas offline;
    [SerializeField] private Text CreateRoomInput;
    [SerializeField] private Text JoinRoomInput;

    // Connection au serveur -------------------------------------------------------------------

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connection au serveur reussie");

        online.gameObject.SetActive(true);
        offline.gameObject.SetActive(false);
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Events des boutons ----------------------------------------------------------------------

    public void OnJoinRandomRoomClicked()
    {
        Debug.Log("Recherche d'une salle...");

        PhotonNetwork.JoinRandomRoom();
    }

    public void OnJoinRoomClicked()
    {
        Debug.Log("Recherche d'une salle...");

        PhotonNetwork.JoinRoom(JoinRoomInput.text);
    }

    public void OnCreateRoomClicked()
    {
        //Si le field est rempli on donne a la salle le nom choisi, sinon on donne un nom random
        string name = (CreateRoomInput.text != "") ? CreateRoomInput.text : "Room" + (int)Random.Range(0, 1000000);

        Debug.Log("Creation d'une salle... " + name);

        CreateRoom(name);
    }

    // Connection aux salles ------------------------------------------------------------------

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Aucune salle trouvee");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Aucune salle trouvee");

        //CreateRoom();
    }

    // Creation de salles ---------------------------------------------------------------------

    private void CreateRoom(string name)
    {
        RoomOptions rops = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = maxPlayers };
        PhotonNetwork.CreateRoom(name, rops);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Salle créée !");
        PhotonNetwork.LoadLevel(1);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Echec de la creation de la salle");

        //CreateRoom();
    }
}
