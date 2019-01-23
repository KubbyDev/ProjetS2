using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class Lobby : MonoBehaviourPunCallbacks
{
    [SerializeField] [Range(1,100)] private byte maxPlayers = 10;

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connection au serveur reussie");

        PhotonNetwork.AutomaticallySyncScene = true;

        Debug.Log("Recherche d'une salle...");

        PhotonNetwork.JoinRandomRoom();
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
        PhotonNetwork.CreateRoom("Room" + (long) Random.Range(long.MinValue, long.MaxValue), rops);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Echec de la creation de la salle");

        CreateRoom();
    }
}
