using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class Room : MonoBehaviourPunCallbacks
{
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        //Cree l'avatar du joueur
        RPC_CreatePlayer();
    }

    //Quand un joueur entre dans la salle
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        //Si ce client est l'hote, on envoie les infos de base sur la partie
        if(PhotonNetwork.IsMasterClient)
            GameDataSync.SendFirstPacket(newPlayer);
    }

    [PunRPC]
    //Cree un avatar pour le joueur
    private void RPC_CreatePlayer()
    {
        PhotonNetwork.Instantiate(Path.Combine("Character", "Player"), transform.position, Quaternion.identity);
    }
}
