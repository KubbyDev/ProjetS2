using System;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;
using UnityEditor;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private Text[] controlsButtonsTexts;   //References aux textes des boutons des controls (les index sont les memes que pour les cles dans GameSettings)
    [SerializeField] private GameObject graphicsMenu;       //Reference a l'onglet Graphics
    [SerializeField] private GameObject controlsMenu;       //Reference a l'onglet Controls
    [SerializeField] private Button graphicsButton;         //Reference au bouton de l'onglet Graphics
    [SerializeField] private Button controlsButton;         //Reference au bouton de l'onglet Controls
    [SerializeField] private Slider[] sensitivity;          //References aux sliders de la sensibilite
    [SerializeField] private InputField[] sensitivityTexts; //References aux valeurs a droite des sliders
    [SerializeField] private Toggle invert;                 //Reference au toggle invert

    [SerializeField] private AudioSource audioSource;       //Reference au systeme de gestion du son        //TODO
    [SerializeField] private Dropdown resolutionDropdown;   //Reference au dropdown des resolutions
    [SerializeField] private Dropdown shadowsDropdown;      //Reference au dropdown des ombres
    [SerializeField] private Dropdown aaDropdown;           //Reference au dropdown de l'anti aliasing
    [SerializeField] private Toggle fullscreenToggle;       //Reference au toggle du fullscreen
    [SerializeField] private Slider volumeSlider;           //Reference au slider du volume

    private Resolution[] resolutions;     //Liste des resolutions que l'ecran peut afficher
    private int currentKey = -1;          //L'index de la touche que l'utilisateur est en train d'assigner

    //  Initialisation  --------------------------------------------------------------------------------------------------------

    void OnEnable()
    {
        RefreshSettings();
    }

    public void RefreshSettings()
    {
        //On rempli le dropdown des resolutions avec toutes les resolutions que l'ecran peut afficher
        resolutionDropdown.options.Clear();
        resolutions = Screen.resolutions.Where(res => res.refreshRate == Screen.currentResolution.refreshRate).ToArray();
        foreach (Resolution res in resolutions)
            resolutionDropdown.options.Add(new Dropdown.OptionData(ToString(res)));

        ApplySettings();
    }

    private static string ToString(Resolution res)
    {
        return res.width + "x" + res.height;
    }

    // Changement de Menu  -----------------------------------------------------------------------------------------------------

    public void OnGraphicsClick()
    {
        graphicsMenu.SetActive(true);
        controlsMenu.SetActive(false);

        graphicsButton.interactable = false;
        controlsButton.interactable = true;
    }

    public void OnControlsClick()
    {
        graphicsMenu.SetActive(false);
        controlsMenu.SetActive(true);

        graphicsButton.interactable = true;
        controlsButton.interactable = false;
    }

    //  Actions des boutons, Menu Graphics  ------------------------------------------------------------------------------------

    //Toggle du fullscreen
    public void OnFullscreenToggle(bool value)
    {
        Settings.settings.fullscreen = value;
    }

    //Dropdown de la resolution
    public void OnResolutionChange(int index)
    {
        Settings.settings.resolutionIndex = index;
    }

    //Dropdown des ombres
    public void OnShadowsChange(int index)
    {
        Settings.settings.shadowsQuality = index;
    }

    //Dropdown de l'AA
    public void OnAntialiasingChange(int index)
    {
        Settings.settings.aaLevel = index;
    }

    //Slider du volume
    public void OnVolumeChange(float volume)
    {
        Settings.settings.volume = volume;
    }

    //  Actions des boutons, Menu Controls  ------------------------------------------------------------------------------------

    public void WaitForKey(int keyIndex)
    {
        if (currentKey == -1)
            currentKey = keyIndex;
    }

    //Cette fonction est appellee plein de fois par frame
    void OnGUI()
    {
        //Si on attend une touche
        if (currentKey != -1)
        {
            Event e = Event.current;
            //Si l'utilisateur a appuye sur une touche
            if (e.isKey || e.isMouse)
            {
                //On change la touche dans les settings et l'affichage
                KeyCode key = e.isMouse ? e.button + KeyCode.Mouse0 : e.keyCode;
                Settings.settings.controls[currentKey] = key;
                controlsButtonsTexts[currentKey].text = key.ToString();

                currentKey = -1;
            }
        }
    }

    void OnDisable()
    {
        currentKey = -1;
    }

    //Changement de sensibilite avec le texte
    public void OnChangeSensitivityX(string value)
    {
        float res;
        if(Single.TryParse(value, out res))
            OnChangeSensitivityX(res);
        else
            OnChangeSensitivityX(Settings.settings.sensitivity[0]);
    }
    
    //Changement de sensibilite avec le texte
    public void OnChangeSensitivityY(string value)
    {
        float res;
        if(Single.TryParse(value, out res))
            OnChangeSensitivityY(res);
        else
            OnChangeSensitivityY(Settings.settings.sensitivity[0]);
    }
    
    //Changement de sensibilite avec le slider
    public void OnChangeSensitivityX(float value)
    {
        Settings.settings.sensitivity[0] = value;             //Enregistrement
        sensitivity[0].value = value;                //Slider
        sensitivityTexts[0].text = value.ToString(); //Texte
    }

    //Changement de sensibilite avec le slider
    public void OnChangeSensitivityY(float value)
    {
        Settings.settings.sensitivity[1] = value;             //Enregistrement
        sensitivity[1].value = value;                //Slider
        sensitivityTexts[1].text = value.ToString(); //Texte
    }

    public void OnToggleInvert(bool value)
    {
        Settings.settings.invertY = value;
    }
    
    //  Enregistrement, chargement et lecture des settings  --------------------------------------------------------------------

    //Bouton Apply
    public void OnApplySettings()
    {
        //Applique les settings dans le jeu
        ApplySettings();

        //Enregistre les settings dans le fichier
        Settings.Save();
    }

    //Applique les settings sur le jeu
    private void ApplySettings()
    {
        //Graphics

        //Mise a jour des options dans Unity
        QualitySettings.antiAliasing = (int) Mathf.Pow(2, Settings.settings.aaLevel);
        //audioSource.volume = settings.volume; TODO
        QualitySettings.shadows = (ShadowQuality) Settings.settings.shadowsQuality;
        Screen.SetResolution(resolutions[Settings.settings.resolutionIndex].width, resolutions[Settings.settings.resolutionIndex].height, Settings.settings.fullscreen);
        
        //Mise a jour de l'affichage
        aaDropdown.value = Settings.settings.aaLevel;
        resolutionDropdown.value = Settings.settings.resolutionIndex;
        shadowsDropdown.value = Settings.settings.shadowsQuality;
        fullscreenToggle.isOn = Settings.settings.fullscreen;
        volumeSlider.value = Settings.settings.volume;

        //Controles
        
        //Mise a jour de l'affichage
        for (int i = 0; i < controlsButtonsTexts.Length; i++)
            controlsButtonsTexts[i].text = Settings.settings.controls[i].ToString();
        OnChangeSensitivityX(Settings.settings.sensitivity[0]);
        OnChangeSensitivityY(Settings.settings.sensitivity[1]);
        invert.isOn = Settings.settings.invertY;
    }
}
