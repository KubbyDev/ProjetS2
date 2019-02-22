using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;

public class TabMenu : MonoBehaviour
{
    private class Item
    {
        public Player player;
        public PlayerInfo infos;
        public Text ping;
        public Text name;
        public Text goals;

        public void UpdateValues()
        {
            ping.text = infos.ping.ToString();    //TODO: Recuperer le ping des autres joueurs
            name.text = player.NickName;
            goals.text = infos.goalsScored.ToString();
        }
    }
    
    [SerializeField] private Image itemPrefab;                  //Un affichage de joueur (une barre contenant le nom et d'autres infos)
    [SerializeField] private VerticalLayoutGroup playerList;    //Le conteneur des items
    [SerializeField] private Text roomName;                     //Le component qui affiche le nom de la salle

    private List<Item> items;
    private float timeToUpdate;    
    
    void Start()
    {
        //On affiche le nom de la salle en haut du menu
        roomName.text = PhotonNetwork.CurrentRoom.Name;
    }

    void Update()
    {
        //On met a jour les valeurs dans le menu toutes les 2 secondes
        if (timeToUpdate > 0)
            timeToUpdate -= Time.deltaTime;
        else
        {
            timeToUpdate = 2;
            UpdateItems();
        }
    }
    
    //Cette fonction est appellee quand l'objet est active (SetActive(true) dans InputManager)
    void OnEnable()
    {
        UpdateList();
    }

    void UpdateItems()
    {
        foreach (Item player in items)
            player.UpdateValues();
    }

    void UpdateList()
    {
        //On detruit tous les fils du vertical layout (on commence a 1 parce qu'en 0 c'est le vertical layout lui meme)
        Transform[] childs = playerList.GetComponentsInChildren<Transform>();
        for (int i = 1; i < childs.Length; i++)
            Destroy(childs[i].gameObject);
        items = new List<Item>();
            
        //Recuperation de tous les joueurs sur le serveur
        Dictionary<int, Player> players = PhotonNetwork.CurrentRoom.Players;

        //Affichage de chaque joueur
        foreach (Player player in players.Values)
        {
            Image element = Instantiate(itemPrefab, playerList.transform);
            if (player.IsLocal) element.color = new Color(0.7f, 0.7f, 0.7f); //On differencie le joueur local

            Item i = new Item
            {
                player = player,
                infos = ((GameObject) player.TagObject).GetComponent<PlayerInfo>(),
                name = element.transform.Find("Name").gameObject.GetComponent<Text>(),
                ping = element.transform.Find("Ping").gameObject.GetComponent<Text>(),
                goals = element.transform.Find("Goals").gameObject.GetComponent<Text>()
            };

            items.Add(i);
        }
        
        UpdateItems();
    }
}
