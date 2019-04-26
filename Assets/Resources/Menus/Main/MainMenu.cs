using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class MainMenu : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject mainMenu;         //Reference au menu principal 
    [SerializeField] private GameObject serversMenu;      //Reference au menu des serveurs
    [SerializeField] private GameObject optionsMenu;      //Reference au menu des options
    [SerializeField] private GameObject heroesMenu;       //Reference au menu des heros

    private ServerSelectionMenu serversMenuScript;        //Reference au script du menu des serveurs
    private bool connected;                               //True: On est connecte au serveur
    private float timeToNextUpdate = 0;                   //Temps restant pour le prochain check de l'etat de la connection
    
    // Connection au serveur -------------------------------------------------------------------

    void Awake()
    {
        //Charge les settings sauvegardes sur le disque dur
        Settings.Load();
        
        //Quand le serveur change de scene, les clients aussi
        PhotonNetwork.AutomaticallySyncScene = true;
        
        serversMenuScript = GameObject.Find("Scripts").GetComponent<ServerSelectionMenu>();
        optionsMenu.transform.Find("Options").GetComponent<OptionsMenu>().RefreshSettings();

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void Update()
    {
        if (timeToNextUpdate > 0)
        {
            timeToNextUpdate -= Time.deltaTime;
            return;
        }
        //Une update toutes les deux secondes
        timeToNextUpdate = 2;

        //Connection
        if (PhotonNetwork.IsConnected && !connected)
        {
            connected = true;
            serversMenuScript.Connected();
        }

        //Deconnection
        if (!PhotonNetwork.IsConnected && connected)
        {
            connected = false;
            serversMenuScript.Disconnected();
        }
        
        //Demande de connection
        if(!PhotonNetwork.IsConnected)
            PhotonNetwork.ConnectUsingSettings();
    }

    // Actions des Boutons ----------------------------------------------------------------------

    public void OnPlayClicked()
    {
        mainMenu.SetActive(false);
        serversMenu.SetActive(true);
    }

    public void OnHeroesClicked()
    {
        mainMenu.SetActive(false);
        heroesMenu.SetActive(true);
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

    //Quand on clique sur Back depuis le menu des serveurs, celui des options ou celui des heros
    public void OnBackClicked()
    {
        mainMenu.SetActive(true);
        serversMenu.SetActive(false);
        heroesMenu.SetActive(false);
        optionsMenu.SetActive(false);
    }
}
