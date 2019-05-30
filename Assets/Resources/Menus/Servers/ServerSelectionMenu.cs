using System.Collections.Generic;
using ExitGames.Client.Photon;
using JetBrains.Annotations;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ServerSelectionMenu : MonoBehaviourPunCallbacks
{
    [SerializeField] private Canvas online;               //Le menu avant la connection au serveur
    [SerializeField] private Canvas offline;              //Le menu apres la connection au serveur
    
    //Room creation
    [SerializeField] private InputField createRoomInput;  //Le texte tappe dans le champ room name de create room
    [SerializeField] private Dropdown teamsSizeDropdown;  //La taille des teams choisies (1v1, 2v2, 3v3, 4v4)
    [SerializeField] private Dropdown maxGoalsDropdown;   //Le nombre de buts max avant que la partie s'arrete
    [SerializeField] private Dropdown maxTimeDropdown;    //La duree de la partie
    [SerializeField] private Toggle enableOvertime;       //La duree de la partie
    [SerializeField] private GameObject createRoomMenu;   //Le menu de creation de room (sans avoir clique sur advanced)
    [SerializeField] private GameObject advCreateRoomMenu;//Le menu de creation de room (apres avoir clique sur advanced)
    
    //Room join
    [SerializeField] private GameObject roomPrefab;       //Le bouton correspondant a une salle
    [SerializeField] private InputField roomNameInput;    //Le texte tappe dans le champ room name de join room
    [SerializeField] private Transform roomList;          //Le cadre dans lequel les rooms s'affichent
    [SerializeField] private GameObject noRoomsText;      //Le texte qui dit no rooms available
    
    //Nickname
    [SerializeField] private InputField nicknameInput;    //Le texte tappe dans le champ nick name
    
    private ErrorMessage errorMessage;                    //Le script qui gere l'affichage des messages d'erreur
    private bool[] selectedGamemodes;                     //Les gamemodes selectionnes dans les filtres             
    private List<RoomInfo> availableRooms;                //La liste des salles existantes

    void Start()
    {
        errorMessage = online.GetComponentInChildren<ErrorMessage>(true);
        selectedGamemodes = new bool[4];
        for (int i = 0; i < 4; i++)
            selectedGamemodes[i] = true;
        availableRooms = new List<RoomInfo>();
        
        //On supprime toutes les salles
        foreach (Transform child in roomList.transform)
            Destroy(child.gameObject);

        //On charge le nickname enregistre
        nicknameInput.text = Settings.settings.nickname;
        PhotonNetwork.LocalPlayer.NickName = Settings.settings.nickname;
    }

    //Quand la connection est etablie
    //Appellee par MainMenu.cs
    public void Connected()
    {
        //Met a jour l'affichage
        online.gameObject.SetActive(true);
        offline.gameObject.SetActive(false);

        PhotonNetwork.JoinLobby();
    }
    
    //Quand la connection est perdue
    //Appellee par MainMenu.cs
    public void Disconnected()
    {
        //Met a jour l'affichage
        online.gameObject.SetActive(false);
        offline.gameObject.SetActive(true);
    }

    void Update()
    {
        //On active le texte no available rooms si aucune salle n'est affichee
        noRoomsText.SetActive(roomList.childCount == 0);
    }

    // Events des boutons ----------------------------------------------------------------------
    
    public void OnJoinRandomRoomClicked()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public void On1v1ToggleClicked(bool value)
    {
        selectedGamemodes[0] = value;
        RefreshRoomsList();
    }
    
    public void On2v2ToggleClicked(bool value)
    {
        selectedGamemodes[1] = value;
        RefreshRoomsList();
    }
    
    public void On3v3ToggleClicked(bool value)
    {
        selectedGamemodes[2] = value;
        RefreshRoomsList();
    }

    public void On4v4ToggleClicked(bool value)
    {
        selectedGamemodes[3] = value;
        RefreshRoomsList();
    }

    public void OnNameFilterChanged()
    {
        RefreshRoomsList();
    }

    public void OnNickNameChanged(string value)
    {
        Settings.settings.nickname = PhotonNetwork.LocalPlayer.NickName = value;
        Settings.Save();
    }
    
    public void OnCreateRoomClicked()
    {
        //Si le champ est rempli on donne a la salle le nom choisi, sinon on donne un nom random
        string text = createRoomInput.text;
        string roomName = text != "" ? text : "Room" + Random.Range(0, 1000000);
        
        //Lis la duree max de partie selectionne
        int maxTime = 0;
        if (maxTimeDropdown.captionText.text != "Unlimited")
        {
            string[] words = maxTimeDropdown.captionText.text.Split(' ');
            maxTime = int.Parse(words[0]) * (words[1] == "min" ? 60 : 1);   
        }
        
        //Lis le nombre de buts selectionne
        int maxGoals = 0;
        if (maxGoalsDropdown.captionText.text != "Unlimited")
            maxGoals = int.Parse(maxGoalsDropdown.captionText.text);
        
        CreateRoom(roomName, teamsSizeDropdown.value +1, maxGoals, maxTime, enableOvertime.isOn);
    }

    public void OnAdvancedClicked()
    {
        advCreateRoomMenu.SetActive(true);
        createRoomMenu.SetActive(false);
    }

    //Quand on clique sur Done depuis les advanced settings
    public void OnDoneClicked()
    {
        advCreateRoomMenu.SetActive(false);
        createRoomMenu.SetActive(true);
    }
     
    public void OnRefreshClicked()
    {
        PhotonNetwork.GetCustomRoomList(TypedLobby.Default, "");
        //RemoveAllRooms();
    }

    public void OnRandomizeNickname()
    {
        Settings.settings.nickname = PhotonNetwork.LocalPlayer.NickName = nicknameInput.text = RandomName.Generate();
        Settings.Save();
    }

    // Liste des salles ------------------------------------------------------------------------

    //Cette methode est appelle par Photon des qu'une salle est cree ou supprimee ou que quelqu'un rejoin ou quitte
    public override void OnRoomListUpdate(List<RoomInfo> newRooms)
    {
        foreach (RoomInfo room in newRooms)
            if (room.MaxPlayers > 0 && room.MaxPlayers != room.PlayerCount)
                if (! availableRooms.Exists(r => r.Name == room.Name))
                    //Si la room n'existe pas encore, on la cree
                    AddRoomToList(room);
                else
                    //Sinon c'est une mise a jour, on met donc a jour ses informations      
                    UpdateRoomInfoInList(room);
            else
                //Si c'est une room qui vient d'etre detruite ou remplie
                RemoveRoomFromList(room);          
    }

    private void AddRoomToList(RoomInfo room)
    {
        availableRooms.Add(room);
        
        if (CorrespondsToFilters(room))
            //On l'ajoute a la liste
            AddToList(room);
    }

    private void RemoveRoomFromList(RoomInfo room)
    {
        //On supprime la salle de la liste
        availableRooms.RemoveAll(r => r.Name == room.Name);    
        
        //On supprime la salle de l'affichage
        foreach (Transform child in roomList.transform)
            if(child.GetChild(0).GetComponent<Text>().text == room.Name)
                Destroy(child.gameObject);
    }

    private void UpdateRoomInfoInList(RoomInfo room)
    {
        //On cherche la salle dans celles qui sont affichees et on met a jour ses informations
        foreach (Transform child in roomList.transform)
            if (child.GetChild(0).GetComponent<Text>().text == room.Name)
            {
                //Nombre de joueurs dans la partie et nombre max de joueurs
                child.GetChild(2).GetComponent<Text>().text = room.PlayerCount + "/" + room.MaxPlayers;
            }
    }

    private bool CorrespondsToFilters(RoomInfo room)
    {
        return room.IsOpen //Si la salle est ouverte
            && room.Name.ToLower().Contains(roomNameInput.text.ToLower()) //Que son nom contient le nom cherche
            && selectedGamemodes[(int) room.CustomProperties["p"] - 1]; //Et que son gamemode est autorise dans les filtres
    }

    private void RefreshRoomsList()
    {
        //On supprime toutes les salles
        RemoveAllRooms();
        
        //On ajoute toutes les salles correspondant aux filtres
        foreach (RoomInfo room in availableRooms)
            if (CorrespondsToFilters(room))
                AddToList(room);
    }

    //Ajoute une salle a l'afficheur
    private void AddToList(RoomInfo room)
    {
        Transform item = Instantiate(roomPrefab, roomList).transform;

        int playerPerTeam = (int) room.CustomProperties["p"];
        item.GetChild(0).GetComponent<Text>().text = room.Name;
        item.GetChild(1).GetComponent<Text>().text = playerPerTeam + "v" + playerPerTeam;
        item.GetChild(2).GetComponent<Text>().text = room.PlayerCount + "/" + room.MaxPlayers;
    }

    private void RemoveAllRooms()
    {
        foreach (Transform child in roomList.transform)
            Destroy(child.gameObject);
    }
    
    // Connection aux salles -------------------------------------------------------------------
    
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        errorMessage.Display("No room found with that name");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        errorMessage.Display("No room found");
    }

    // Creation de salles ---------------------------------------------------------------------

    private static void CreateRoom(string name, int playersPerTeam, int maxGoals, int gameDurationInSec, bool enableOvertime)
    {
        Hashtable gameConfig = new Hashtable
        {
            {"p", playersPerTeam}, 
            {"g", maxGoals},
            {"d", gameDurationInSec},
            {"o", enableOvertime}
        };

        RoomOptions rops = new RoomOptions
        {
            IsVisible = true,                            //Je sais pas a quoi ca sert, mais faut le mettre a true je pense
            IsOpen = true,                               //Si la salle est joinable
            MaxPlayers = (byte) (playersPerTeam*2),      //Le nombre max de joueurs dans la salle
            CustomRoomProperties = gameConfig,           //La liste des parametres a enregistrer avec la salle
            CustomRoomPropertiesForLobby = new []{"p"},  //La liste des parametres qui doivent etre transmis pour la liste des salles disponibles
            CleanupCacheOnLeave = false                  //Empeche les objets lies a tel ou tel joueur de disparaitre
        };
        
        PhotonNetwork.CreateRoom(name, rops);
    }

    public override void OnCreatedRoom()
    {
        //On charge la map
        PhotonNetwork.LoadLevel(1);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorMessage.Display("Room creation failed");
    }
}
