using Photon.Pun;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject optionsMenu;

    private InputManager inputs;

    public void SetInputManager(InputManager inputManager)
    {
        inputs = inputManager;
    }

    public void OnResumeClick()
    {
        inputs.TogglePauseMenu();
    }

    public void OnOptionsClick()
    {
        this.gameObject.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void OnQuitClick()
    {

    }

    //Quand on clique sur Back depuis le menu des options
    public void OnBackClicked()
    {
        this.gameObject.SetActive(true);
        optionsMenu.SetActive(false);
    }
}
