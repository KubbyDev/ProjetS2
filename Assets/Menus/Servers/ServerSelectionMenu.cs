using System.Collections.Generic;
using ExitGames.Client.Photon;
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
    
    //Room join
    [SerializeField] private GameObject roomPrefab;       //Le bouton correspondant a une salle
    [SerializeField] private InputField roomNameInput;    //Le texte tappe dans le champ room name de join room
    [SerializeField] private Transform roomList;          //Le cadre dans lequel les rooms s'affichent
    [SerializeField] private GameObject noRoomsText;      //Le texte qui dit no rooms available
    
    private ErrorMessage errorMessage;                    //Le script qui gere l'affichage des messages d'erreur
    private bool[] selectedGamemodes;                     //Les gamemodes selectionnes dans les filtres             
    private List<RoomInfo> availableRooms;                //La liste des salles existantes

    void Start()
    {
        errorMessage = online.GetComponentInChildren<ErrorMessage>(true);
        selectedGamemodes = new bool[5];
        for (int i = 0; i < 5; i++)
            selectedGamemodes[i] = true;
        availableRooms = new List<RoomInfo>();
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

    // Events des boutons ----------------------------------------------------------------------

    public void OnJoinRandomRoomClicked()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public void OnRefreshClicked()
    {
        RefreshRoomsList();
    }

    public void On1v1ToggleClicked(bool value)
    {
        selectedGamemodes[0] = value;
        RefreshRoomsList();
    }
    
    public void On2v2ToggleClicked(bool value)
    {
        selectedGamemodes[1] = value;
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
    
    public void OnSpecialToggleClicked(bool value)
    {
        selectedGamemodes[4] = value;
        RefreshRoomsList();
    }

    public void OnNameFilterChanged()
    {
        RefreshRoomsList();
    }
    
    public void OnCreateRoomClicked()
    {
        //Si le champ est rempli on donne a la salle le nom choisi, sinon on donne un nom random
        string text = createRoomInput.text;
        string roomName = text != "" ? text : "Room" + Random.Range(0, 1000000);

        CreateRoom(roomName, teamsSizeDropdown.value +1, int.MaxValue, 5*60);
    }

    // Connection aux salles ------------------------------------------------------------------

    public override void OnRoomListUpdate(List<RoomInfo> newRoomList)
    {
        //TODO utiliser ca pour optimiser le refresh      
        foreach (RoomInfo room in newRoomList)
            if (room.MaxPlayers > 0)
                //Si c'est une room qui vient d'etre creee
                availableRooms.Add(room);
            else
                //Si c'est une room qui vient d'etre detruite
                availableRooms.RemoveAll(r => r.Name == room.Name);
        
        RefreshRoomsList();
    }

    public void RefreshRoomsList()
    {
        //On supprime toutes les salles
        foreach (Transform child in roomList.transform)
            Destroy(child.gameObject);
        
        //On ajoute toutes les salles correspondant aux filtres
        foreach (RoomInfo room in availableRooms)
            if (room.IsOpen //Si la salle est ouverte
                && room.Name.Contains(roomNameInput.text) //Que son nom contient le nom cherche
                && selectedGamemodes[(int) room.CustomProperties["p"] - 1]
                ) //Et que son gamemode est autorise dans les filtres
                //On l'ajoute a la liste
                AddToList(room);

        //On active le texte no available rooms si aucune salle n'est trouvee
        noRoomsText.SetActive(roomList.childCount == 0);
    }

    private void AddToList(RoomInfo room)
    {
        Transform item = Instantiate(roomPrefab, roomList).transform;

        int playerPerTeam = (int) room.CustomProperties["p"];
        item.GetChild(0).GetComponent<Text>().text = room.Name;
        item.GetChild(1).GetComponent<Text>().text = playerPerTeam + "v" + playerPerTeam;
        item.GetChild(2).GetComponent<Text>().text = room.PlayerCount + "/" + room.MaxPlayers;
    }
    
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        errorMessage.Display("No room found with that name");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        errorMessage.Display("No room found");
    }

    // Creation de salles ---------------------------------------------------------------------

    private static void CreateRoom(string name, int playersPerTeam, int maxGoals, int gameDurationInSec)
    {
        Hashtable gameConfig = new Hashtable
        {
            {"p", playersPerTeam}, 
            {"g", maxGoals},
            {"d", gameDurationInSec}
        };

        RoomOptions rops = new RoomOptions
        {
            IsVisible = true, 
            IsOpen = true, 
            MaxPlayers = (byte) (playersPerTeam*2), 
            CustomRoomProperties = gameConfig, 
            CustomRoomPropertiesForLobby = new []{"p"}
        };
        
        PhotonNetwork.CreateRoom(name, rops);
    }

    public override void OnCreatedRoom()
    {
        //On charge le BasicField
        PhotonNetwork.LoadLevel(1);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorMessage.Display("Room creation failed");
    }
}
