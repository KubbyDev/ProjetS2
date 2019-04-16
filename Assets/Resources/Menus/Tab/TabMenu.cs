using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TabMenu : MonoBehaviour
{
    //Chaque objet correspond a un rectangle
    private class Item
    {
        public PlayerInfo infos;
        public Text ping;
        public Text goals;

        public void UpdateValues()
        {
            ping.text = infos.ping.ToString();
            goals.text = infos.goalsScored.ToString();
        }
    }

    public static TabMenu script;
    
    [SerializeField] private Image itemPrefab;                    //Un affichage de joueur (une barre contenant le nom et d'autres infos)
    [SerializeField] private VerticalLayoutGroup playerList;      //La liste generale (pour la pregame)
    [SerializeField] private VerticalLayoutGroup bluePlayerList;  //La liste des joueurs bleus
    [SerializeField] private VerticalLayoutGroup orangePlayerList;//La liste des joueurs oranges
    [SerializeField] private Text roomName;                       //Le component qui affiche le nom de la salle

    private List<Item> items = new List<Item>();
    private float timeToUpdate;
    private float timeToUpdateValues;

    private void Awake()
    {
        script = this;
    }

    void Start()
    {
        //On affiche le nom de la salle en haut du menu
        roomName.text = PhotonNetwork.CurrentRoom.Name;
    }

    //Cette fonction sert a faire passer le menu du mode non separe au mode separe (les teams sont separees)
    public void SeparateTeams()
    {
        playerList.gameObject.SetActive(false);
        orangePlayerList.gameObject.SetActive(true);
        bluePlayerList.gameObject.SetActive(true);
    }

    void Update()
    {
        //On met a jour les valeurs dans le menu toutes les 0.2 secondes
        if (timeToUpdateValues < 0)
            timeToUpdateValues -= Time.deltaTime;
        else
        {
            timeToUpdateValues = 0.2f;
            UpdateItems();
        }
        
        //On met a jour le menu tout entier toutes les secondes
        if (timeToUpdate < 0)
            timeToUpdate -= Time.deltaTime;
        else
        {
            timeToUpdate = 1;
            UpdateList();
        }
    }

    //Cette fonction met a jour les valeurs sans reconstruire la liste
    private void UpdateItems()
    {
        foreach (Item player in items)
            player.UpdateValues();
    }

    //Cette fonction reconstruit la liste
    public void UpdateList()
    {
        //On detruit tous les fils des vertical layout
        foreach (Transform child in bluePlayerList.transform)
            Destroy(child.gameObject);
        foreach (Transform child in orangePlayerList.transform)
            Destroy(child.gameObject);
        foreach (Transform child in playerList.transform)
            Destroy(child.gameObject);
        items = new List<Item>();

        //On traite chaque joueur par ordre de nombre de buts
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player")
            .OrderByDescending(player => player.GetComponent<PlayerInfo>().goalsScored))
        {
            //Ajoute le player a la bonne liste en fonction de sa team
            AddPlayerToList(player);   
        }
           
        UpdateItems();
    }

    public void AddPlayerToList(GameObject player)
    {
        PlayerInfo infos = player.GetComponent<PlayerInfo>();
        Team team = infos.team;
        
        Transform chosenList = playerList.transform;
        if(team != Team.None)
            chosenList = (team == Team.Blue ? bluePlayerList : orangePlayerList).transform;
                
        Image element = Instantiate(itemPrefab, chosenList);
        
        //On met l'item de la couleur de la team
        Color color = team == Team.None 
            ? new Color(0.4f, 0.4f, 0.4f) 
            : team == Team.Blue 
                ? new Color(0.0f, 0.6f, 1.0f) 
                : new Color(0.95f, 0.6f, 0.1f);
            
        //On differencie le joueur local
        if (player == PlayerInfo.localPlayer)
            color = new Color(color[0] + (1 - color[0])*0.5f, color[1] + (1 - color[1])*0.5f, color[2] + (1 - color[2])*0.5f);

        element.color = color;
        element.transform.Find("Name").gameObject.GetComponent<Text>().text = infos.nickname;
            
        items.Add(new Item
        {
            infos = infos,
            ping = element.transform.Find("Ping").gameObject.GetComponent<Text>(),
            goals = element.transform.Find("Goals").gameObject.GetComponent<Text>()
        });
    }
}
