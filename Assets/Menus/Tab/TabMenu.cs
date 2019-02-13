using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabMenu : MonoBehaviour
{
    [SerializeField] private Image item;                        //Un affichage de joueur (une barre contenant le nom et d'autres infos)
    [SerializeField] private VerticalLayoutGroup playerList;    //Le conteneur des items
    [SerializeField] private Text roomName;                     //Le component qui affiche le nom de la salle

    void Start()
    {
        //On affiche le nom de la salle en haut du menu
        roomName.text = PhotonNetwork.CurrentRoom.Name;
    }

    //Cette fonction est appellee quand l'objet est active (SetActive(true) dans InputManager)
    void OnEnable()
    {
        //On detruit tous les fils du vertical layout (on commence a 1 parce qu'en 0 c'est le vertical layout lui meme)
        Transform[] childs = playerList.GetComponentsInChildren<Transform>();
        for (int i = 1; i < childs.Length; i++)
            Destroy(childs[i].gameObject);

        //Recuperation de tous les joueurs sur le serveur
        Dictionary<int, Player> players = PhotonNetwork.CurrentRoom.Players;

        //Affichage de chaque joueur
        foreach (Player player in players.Values)
        { 
            Image element = Instantiate(item);
            element.GetComponentInChildren<Text>().text = player.NickName;
            if (player.IsLocal) element.color = new Color(0.7f, 0.7f, 0.7f); //On differencie le joueur local
            element.transform.SetParent(playerList.transform);
        }
    }
}
