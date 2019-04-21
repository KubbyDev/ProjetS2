﻿using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;

//Ce script gere les entrees et sorties de la salle

public class Room : MonoBehaviourPunCallbacks
{
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        //Cree l'avatar du joueur
        PhotonNetwork.Instantiate("Character/Player", transform.position, Quaternion.identity);
    }

    //Quand un joueur entre dans la salle
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        //Si ce client est l'hote, on envoie les infos de base sur la partie
        if (PhotonNetwork.IsMasterClient)
            GameDataSync.SendFirstPacket(newPlayer);
        
        //Tout le monde donne ses infos au nouvel arrivant
        foreach (PlayerInfo player in GameObject.FindGameObjectsWithTag("Player")
                                     .Select(player => player.GetComponent<PlayerInfo>()))
        {
            player.UpdateInfos(newPlayer);   
        }
    }
    
    //Quand un joueur quitte on le remplace par une IA
    public override void OnPlayerLeftRoom(Player player)
    {
        base.OnPlayerLeftRoom(player);
        
        //A moins que ce soit le host, que la game ait deja demarre et qu'elle ne soit pas finie, on ne fait rien
        if (! (PhotonNetwork.IsMasterClient && PreGameManager.script.gameStarting && !GameManager.gameFinished))
            return;
        
        //Si c'est le host et que la game a deja demarre
        //on cree une IA pour remplacer le joueur qui vient de quitter
        
        PlayerInfo oldPlayerInfo = ((GameObject) player.TagObject).GetComponent<PlayerInfo>();
        
        PlayerInfo newIaInfos = PhotonNetwork.Instantiate(Path.Combine("AI", "AI"), oldPlayerInfo.transform.position, oldPlayerInfo.transform.rotation).GetComponent<PlayerInfo>();
        newIaInfos.GetComponent<PlayerInfo>().SetTeam(oldPlayerInfo.team);
        newIaInfos.GetComponent<PlayerInfo>().UpdateInfos();
        newIaInfos.GetComponent<Skills>().timeToMove = GameManager.timeLeftForKickoff; //Bloque l'IA si elle rejoint pendant un engagement
    }
}
