using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class RoomButton : MonoBehaviour
{
    public void OnClicked()
    {
        //Rejoint la salle (son nom est stockee dans le texte Name, donc on va le chercher la)
        PhotonNetwork.JoinRoom(transform.GetChild(0).GetComponent<Text>().text);
    }
}
