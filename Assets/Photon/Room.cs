﻿using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

//Ce script gere les entrees et sorties de la salle

public class Room : MonoBehaviourPunCallbacks
{
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        //Cree l'avatar du joueur
        PhotonNetwork.Instantiate(Path.Combine("Character", "Player"), transform.position, Quaternion.identity);
    }

    //Quand un joueur entre dans la salle
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        //Si ce client est l'hote, on envoie les infos de base sur la partie
        if (PhotonNetwork.IsMasterClient)
            GameDataSync.SendFirstPacket(newPlayer);
    }
    
    //Quand un joueur quitte on le remplace par une IA
    public override void OnPlayerLeftRoom(Player player)
    {
        base.OnPlayerLeftRoom(player);
        
        if (!PhotonNetwork.IsMasterClient)
            return;
        
        PlayerInfo oldPlayerInfo = ((GameObject) player.TagObject).GetComponent<PlayerInfo>();
        
        PlayerInfo newIaInfos = PhotonNetwork.Instantiate(Path.Combine("AI", "AI"), oldPlayerInfo.transform.position, oldPlayerInfo.transform.rotation).GetComponent<PlayerInfo>();
        newIaInfos.GetComponent<PlayerInfo>().SetTeam(oldPlayerInfo.team);
        newIaInfos.GetComponent<PlayerInfo>().UpdateInfos();
    }
}
