using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject optionsMenu;

    public void OnResumeClick()
    {
        this.gameObject.SetActive(false);
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
