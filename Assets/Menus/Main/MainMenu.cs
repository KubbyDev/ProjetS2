﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject serversMenu;
    [SerializeField] private GameObject optionsMenu;

    private ServerSelectionMenu serversMenuScript;
    //private OptionsMenu optionsMenuScript; Existe pas encore                                           //TODO

    // Connection au serveur -------------------------------------------------------------------

    void Start()
    {
        //Demande de connection
        PhotonNetwork.ConnectUsingSettings();

        serversMenuScript = GameObject.Find("Scripts").GetComponent<ServerSelectionMenu>();
        //optionsMenuScript = GameObject.Find("Scripts").GetComponent<OptionsMenu>();                    //TODO
    }

    //Quand la connection est etablie
    public override void OnConnectedToMaster()
    {
        //On cache le menu offline et on montre le menu online
        serversMenuScript.Connected();

        //Quand le serveur change de scene, les clients aussi
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void OnPlayClicked()
    {
        mainMenu.SetActive(false);
        serversMenu.SetActive(true);
    }

    public void OnOptionsClicked()
    {
        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void OnQuitClicked()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

    //Quand on clique sur Back depuis le menu des serveurs ou celui des options
    public void OnBackClicked()
    {
        mainMenu.SetActive(true);
        serversMenu.SetActive(false);
        optionsMenu.SetActive(false);
    }
}
