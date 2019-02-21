using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PreGameManager : MonoBehaviour
{
    [SerializeField] private int playersToStart = 1;

    private bool gameStarted = false;
    
    void Update()
    {
        if (!gameStarted && PhotonNetwork.CurrentRoom.PlayerCount >= playersToStart)
        {
            GameObject.Find("GameManager").GetComponent<GameManager>().StartGame();
            gameStarted = true;
        }
    }
}
