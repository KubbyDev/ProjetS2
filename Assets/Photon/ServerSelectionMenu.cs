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
    [SerializeField] private Button quickPlay;
    [SerializeField] private Text connectingText;

    // Connection au serveur -------------------------------------------------------------------
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connection au serveur reussie");

        quickPlay.gameObject.SetActive(true);
        connectingText.gameObject.SetActive(false);
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Quick Play ------------------------------------------------------------------------------
    public void OnQuickPlayButtonClicked()
    {
        Debug.Log("Recherche d'une salle...");

        PhotonNetwork.JoinRandomRoom();
    }

    public void OnCancelButtonClicked()
    {
        
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Aucune salle trouvee");

        CreateRoom();
    }

    private void CreateRoom()
    {
        Debug.Log("Creation d'une salle...");

        RoomOptions rops = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = maxPlayers };
        PhotonNetwork.CreateRoom("Room" + (int) Random.Range(0, 1000000), rops);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Salle créée !");
        PhotonNetwork.LoadLevel(1);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Echec de la creation de la salle");

        CreateRoom();
    }
}
