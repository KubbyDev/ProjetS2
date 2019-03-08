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
        public PlayerInfo infos;
        public Text ping;
        public Text name;
        public Text goals;

        public void UpdateValues()
        {
            ping.text = infos.ping.ToString();
            name.text = infos.gameObject.name;
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

        //Affichage de chaque joueur
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            Image element = Instantiate(itemPrefab, playerList.transform);
            
            //On met le carre de la couleur de la team du joueur
            element.color = player.GetComponent<PlayerInfo>().team == Team.Blue ? new Color(0.0f, 0.6f, 1.0f) : new Color(0.95f, 0.6f, 0.1f);
            
            //On differencie le joueur local
            if (player == PlayerInfo.localPlayer) 
                element.color = new Color(0.9f, 0.9f, 0.9f);

            Item i = new Item
            {
                infos = player.GetComponent<PlayerInfo>(),
                name = element.transform.Find("Name").gameObject.GetComponent<Text>(),
                ping = element.transform.Find("Ping").gameObject.GetComponent<Text>(),
                goals = element.transform.Find("Goals").gameObject.GetComponent<Text>()
            };

            items.Add(i);
        }
        
        UpdateItems();
    }
}
