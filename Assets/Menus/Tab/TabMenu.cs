using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TabMenu : MonoBehaviour
{
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
    [SerializeField] private VerticalLayoutGroup bluePlayerList;  //La liste des joueurs bleus
    [SerializeField] private VerticalLayoutGroup orangePlayerList;//La liste des joueurs oranges
    [SerializeField] private Text roomName;                       //Le component qui affiche le nom de la salle

    private List<Item> items;
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

    void Update()
    {
        //On met a jour les valeurs dans le menu toutes les secondes
        if (timeToUpdateValues < 0)
            timeToUpdateValues -= Time.deltaTime;
        else
        {
            timeToUpdateValues = 1;
            UpdateItems();
        }
        
        //On met a jour le menu tout entier toutes les 5 secondes
        if (timeToUpdate < 0)
            timeToUpdate -= Time.deltaTime;
        else
        {
            timeToUpdate = 5;
            UpdateList();
        }
    }
    
    //Cette fonction est appellee quand l'objet est active (SetActive(true) dans InputManager)
    void OnEnable()
    {
        UpdateList();
    }

    private void UpdateItems()
    {
        foreach (Item player in items)
            player.UpdateValues();
    }

    public void UpdateList()
    {
        //On detruit tous les fils des vertical layout
        foreach (Transform child in bluePlayerList.transform)
            Destroy(child.gameObject);
        foreach (Transform child in orangePlayerList.transform)
            Destroy(child.gameObject);
        items = new List<Item>();

        foreach (Team team in Teams.Each())
        {
            //Affichage de chaque joueur de la team en cours de traitement
            //(On traite a chaque fois chaque joueur de la bonne Team dans le bon ordre)
            foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player")
                .Where(player => player.GetComponent<PlayerInfo>().team == team)
                .OrderByDescending(player => player.GetComponent<PlayerInfo>().goalsScored))
            {
                //On instancie l'image en tant que fils du bon layout group (bleu ou orange)
                Image element = Instantiate(itemPrefab, (team == Team.Blue ? bluePlayerList : orangePlayerList).transform);

                //On met l'item de la couleur de la team
                Color color = team == Team.Blue ? new Color(0.0f, 0.6f, 1.0f) : new Color(0.95f, 0.6f, 0.1f);
                PlayerInfo infos = player.GetComponent<PlayerInfo>();
                
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
           
        UpdateItems();
    }
}
