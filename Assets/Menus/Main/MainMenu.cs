using Photon.Pun;
using UnityEngine;

public class MainMenu : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject mainMenu;         //Reference au menu principal 
    [SerializeField] private GameObject serversMenu;      //Reference au menu des serveurs
    [SerializeField] private GameObject optionsMenu;      //Reference au menu des options

    private ServerSelectionMenu serversMenuScript;        //Reference au script du menu des serveurs

    // Connection au serveur -------------------------------------------------------------------

    void Start()
    {
        //Demande de connection
        PhotonNetwork.ConnectUsingSettings();

        serversMenuScript = GameObject.Find("Scripts").GetComponent<ServerSelectionMenu>();
    }

    //Quand la connection est etablie
    public override void OnConnectedToMaster()
    {
        //On cache le menu offline et on montre le menu online
        serversMenuScript.Connected();

        //Quand le serveur change de scene, les clients aussi
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Actions des Boutons ----------------------------------------------------------------------

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
