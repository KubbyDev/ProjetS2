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
        SceneManager.LoadScene(0);
    }

    //Quand on clique sur Back depuis le menu des options
    public void OnBackClicked()
    {
        this.gameObject.SetActive(true);
        optionsMenu.SetActive(false);
    }
}
