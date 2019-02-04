using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class Room : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
    private int playerNumber;    //Le nombre de joueurs dans la salle

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        //Met a jour le nombre de joueurs dans la salle
        playerNumber = PhotonNetwork.CurrentRoom.PlayerCount;

        //Le pseudo du joueur dans la salle
        PhotonNetwork.NickName = "Player" + playerNumber.ToString();

        //Cree l'avatar du joueur
        RPC_CreatePlayer();
    }

    //Quand un joueur entre dans la salle
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        playerNumber++;
    }

    [PunRPC]
    //Cree un avatar pour le joueur
    private void RPC_CreatePlayer()
    {
        PhotonNetwork.Instantiate(Path.Combine("Character", "Player"), transform.position, Quaternion.identity);
    }

    //Renvoie le nombre de joueurs dans la salle
    public int getPlayerNumber()
    {
        return playerNumber;
    }
}
