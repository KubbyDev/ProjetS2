using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject mainMenu;         //Reference au menu principal 
    [SerializeField] private GameObject serversMenu;      //Reference au menu des serveurs
    [SerializeField] private GameObject optionsMenu;      //Reference au menu des options
    [SerializeField] private GameObject heroesMenu;       //Reference au menu des heros
    [SerializeField] private GameObject messagesMenu;     //Reference au menu qui affiche des messages (crash co par exemple)

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
        
        //Limite les FPS a 100 sinon ca cause des problemes
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 100;
        
        serversMenuScript = GameObject.Find("Scripts").GetComponent<ServerSelectionMenu>();
        optionsMenu.transform.Find("Options").GetComponent<OptionsMenu>().RefreshSettings();
        
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        //Si il y a un message a afficher (par exemple perte de connection avec le serveur)
        if (Tools.message != null)
        {
            messagesMenu.SetActive(true);
            messagesMenu.transform.Find("Menu").Find("Text").GetComponent<Text>().text = Tools.message;
            Tools.message = null;
        }
    }
    
    public override void OnConnectedToMaster()
    {
        connected = true;
        serversMenuScript.Connected();
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

        //Deconnection
        if (!PhotonNetwork.IsConnectedAndReady && connected)
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
    
    //Le bouton OK de l'afficheur de message (perte de connection avec le serveur par exemple)
    public void OnOkClicked()
    {
        messagesMenu.SetActive(false);
    }
}
