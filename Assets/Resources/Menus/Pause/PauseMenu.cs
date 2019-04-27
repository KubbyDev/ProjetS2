using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject optionsMenu;

    public void OnResumeClick()
    {
        //Ferme le menu
        PlayerInfo.localPlayer.GetComponent<InputManager>().TogglePauseMenu();
    }

    public void OnOptionsClick()
    {
        this.gameObject.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void OnQuitClick()
    {
        PhotonNetwork.LeaveRoom();
        //Le chargement de la scene du menu principal se fait dans OnLeftRoom() dans Room.cs
    }

    //Quand on clique sur Back depuis le menu des options
    public void OnBackClicked()
    {
        this.gameObject.SetActive(true);
        optionsMenu.SetActive(false);
    }
}
