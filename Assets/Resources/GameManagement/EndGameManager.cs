﻿using System.Collections;
using System.Linq;
using Photon.Pun;
using UnityEngine;

public class EndGameManager : MonoBehaviour
{
    public static EndGameManager script;
    
    private Position[] winPositions;  //Liste des positions possibles sur le podium des gagnants (il y en a 8 pour gerer les egalites)
    private Position[] losePositions; //Liste des positions possibles sur le podium des perdants (il y en a 4)
    private Transform podium;
    
    void Awake()
    {
        script = this;

        podium = GameObject.Find("Podium").transform;
        Transform podiumWin = podium.Find("Winners");
        Transform podiumLose = podium.Find("Losers");

        //Recuperation de toutes les positions possibles sur le podium
        winPositions = new Position[8];
        losePositions = new Position[4];
        for(int i = 0; i < 8; i++)
            winPositions[i] = new Position(podiumWin.GetChild(i));
        for(int i = 0; i < 4; i++)
            losePositions[i] = new Position(podiumLose.GetChild(i));
    }
    
    public void EndGame(Team losingTeam)
    {
        StartCoroutine(EndGameCoroutine(losingTeam));
    }

    IEnumerator EndGameCoroutine(Team losingTeam)
    {
        yield return new WaitForSeconds(3);
        MakePodium(losingTeam);
    }

    private void MakePodium(Team losingTeam)
    {
        //Met la camera au bon endroit
        Transform cameraPosition = podium.Find("CameraPosition");
        Camera.main.transform.SetPositionAndRotation(cameraPosition.position, cameraPosition.rotation);
        
        int winFilling = 0;
        int loseFilling = 0;
        
        //Pour chaque joueur par ordre de buts, puis par ordre de ViewID (identifiant sur le reseau)
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player")
            .OrderByDescending(p => p.GetComponent<PlayerInfo>().goalsScored)
            .ThenBy(p => p.GetComponent<PhotonView>().ViewID))
        {
            PlayerInfo playerInfos = player.GetComponent<PlayerInfo>();

            //On selectionne la meilleure position libre
            //Soit dans la partie losing soit dans la partie wining
            //En fontion de la team du joueur
            Position position;
            if (playerInfos.team == losingTeam)
            {
                position = losePositions[loseFilling];
                loseFilling++;
            }
            else
            {
                position = winPositions[winFilling];
                winFilling++;
            }

            //On place le mesh du joueur et son pseudo a cette position
            position.meshFilter.GetComponent<MeshRenderer>().enabled = true; //playerInfos.hero.GetMesh(); //TODO
            //position.nickname.text = playerInfos.nickname;  //TODO
            
            //Detruit le player pour eviter tout probleme
            Destroy(player);
        }
    }
    
    //Cette classe represente une position sur le podium
    //Elle permet de donner un acces rapide aux trucs a modifier quand on veut
    //Afficher un joueur sur le podium
    private class Position
    {
        public MeshFilter meshFilter;
        //public TextMesh nickname;

        public Position(Transform podiumPosition)
        {
            meshFilter = podiumPosition.GetComponent<MeshFilter>();
            //nickname = podiumPosition.Find("Nickname").GetComponent<TextMesh>();
        }
    }
}