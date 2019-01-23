using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class Room : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
    private int playerNumber;

    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.AddCallbackTarget(this);

        //On ajoute un EventListener qui execute cette fonction des qu'une scene a fini de charger
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.RemoveCallbackTarget(this);
        SceneManager.sceneLoaded -= OnSceneFinishedLoading;
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("Salle rejointe");

        PhotonNetwork.NickName = playerNumber.ToString();

        RPC_CreatePlayer();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        Debug.Log("Un joueur a rejoint la salle");
        playerNumber++;
    }

    void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {

    }

    [PunRPC]
    private void RPC_CreatePlayer()
    {
        Debug.Log("Creation d'un joueur");

        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player"), transform.position, Quaternion.identity);
    }

    public int getPlayerNumber()
    {
        return playerNumber;
    }
}
