using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class RoomButton : MonoBehaviour
{
    public void OnClicked()
    {
        PhotonNetwork.JoinRoom(transform.GetChild(0).GetComponent<Text>().text);
    }
}
