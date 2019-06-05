using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class OptionsMenu : MonoBehaviour
{
    [Header("Tabs")]
    [SerializeField] private GameObject graphicsMenu;       //Reference a l'onglet Graphics
    [SerializeField] private GameObject controlsMenu;       //Reference a l'onglet Controls
    [SerializeField] private GameObject audioMenu;          //Reference a l'onglet Audio
    [SerializeField] private Button graphicsButton;         //Reference au bouton de l'onglet Graphics
    [SerializeField] private Button controlsButton;         //Reference au bouton de l'onglet Controls
    [SerializeField] private Button audioButton;            //Reference au bouton de l'onglet Audio
    [Header("Graphics")]
    [SerializeField] private Dropdown resolutionDropdown;   //Reference au dropdown des resolutions
    [SerializeField] private Dropdown shadowsDropdown;      //Reference au dropdown des ombres
    [SerializeField] private Dropdown reflectionsDropdown;  //Reference au dropdown des reflections
    [SerializeField] private Dropdown aaDropdown;           //Reference au dropdown de l'anti aliasing
    [SerializeField] private Toggle fullscreenToggle;       //Reference au toggle du fullscreen
    [Header("Controls")]
    [SerializeField] private Text[] controlsButtonsTexts;   //References aux textes des boutons des controls (les index sont les memes que pour les cles dans GameSettings)
    [SerializeField] private Slider[] sensitivity;          //References aux sliders de la sensibilite
    [SerializeField] private InputField[] sensitivityTexts; //References aux valeurs a droite des sliders
    [SerializeField] private Toggle invert;                 //Reference au toggle invert
    [Header("Audio")]
    [SerializeField] private AudioMixer masterVolume;       //Reference au systeme de gestion du son
    [SerializeField] private Slider volumeSlider;           //Reference au slider du volume
 
    private Resolution[] resolutions;     //Liste des resolutions que l'ecran peut afficher
    private int currentKey = -1;          //L'index de la touche que l'utilisateur est en train d'assigner

    //  Initialisation  --------------------------------------------------------------------------------------------------------

    //Quand le menu est ouvert, on refresh les afficheurs
    void OnEnable()
    {
        currentKey = -1;
        RefreshSettings();
        SceneManager.activeSceneChanged += ReflectionsQuality.OnLoadLevel;
    }

    public void RefreshSettings()
    {
        //On rempli le dropdown des resolutions avec toutes les resolutions que l'ecran peut afficher
        resolutionDropdown.options.Clear();
        resolutions = Screen.resolutions.Where(res => res.refreshRate == Screen.currentResolution.refreshRate).Reverse().ToArray();
        foreach (Resolution res in resolutions)
            resolutionDropdown.options.Add(new Dropdown.OptionData(ToString(res)));

        //Met a jour les affichages et les parametres du jeu
        DisplaySettings();
        ApplySettings();
    }

    private static string ToString(Resolution res)
    {
        return res.width + "x" + res.height;
    }

    // Changement de Menu  -----------------------------------------------------------------------------------------------------

    private void ToggleTab(Button tabButton, GameObject tab, bool selected)
    {
        tab.SetActive(selected);

        tabButton.interactable = !selected;
        tabButton.GetComponent<Image>().color = selected ? new Color(1,1,1,1) : new Color(0.6f,0.6f,0.6f,1);
        tabButton.transform.Find("Text").GetComponent<Text>().color = selected ? new Color(1,1,1,1) : new Color(0.6f,0.6f,0.6f,1);
    }

    public void OnGraphicsClick()
    {
        ToggleTab(graphicsButton, graphicsMenu, true);
        ToggleTab(controlsButton, controlsMenu, false);
        ToggleTab(audioButton, audioMenu, false);
    }

    public void OnControlsClick()
    {
        ToggleTab(graphicsButton, graphicsMenu, false);
        ToggleTab(controlsButton, controlsMenu, true);
        ToggleTab(audioButton, audioMenu, false);
    }
    
    public void OnAudioClick()
    {
        ToggleTab(graphicsButton, graphicsMenu, false);
        ToggleTab(controlsButton, controlsMenu, false);
        ToggleTab(audioButton, audioMenu, true);
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
                controlsButtonsTexts[currentKey].text = key.ToString();

                currentKey = -1;
            }
        }
    }

    //Changement de sensibilite avec le texte
    public void OnChangeSensitivityX(string value)
    {
        if(float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out float res))
            OnChangeSensitivityX(res);
        else
            OnChangeSensitivityX(Settings.settings.sensitivity[0]);
    }
    
    //Changement de sensibilite avec le texte
    public void OnChangeSensitivityY(string value)
    {
        if(float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out float res))
            OnChangeSensitivityY(res);
        else
            OnChangeSensitivityY(Settings.settings.sensitivity[0]);
    }
    
    //Changement de sensibilite avec le slider
    public void OnChangeSensitivityX(float value)
    {
        sensitivity[0].value = value;                //Slider
        sensitivityTexts[0].text = value.ToString(CultureInfo.InvariantCulture.NumberFormat); //Texte
    }

    //Changement de sensibilite avec le slider
    public void OnChangeSensitivityY(float value)
    {
        sensitivity[1].value = value; //Slider
        sensitivityTexts[1].text = value.ToString(CultureInfo.InvariantCulture.NumberFormat); //Texte
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

    //Applique les settings sur le jeu en fontion des elements dans le menu
    private void ApplySettings()
    {
        //Graphics
        QualitySettings.antiAliasing = (int) Mathf.Pow(2, aaDropdown.value);
        Settings.settings.aaLevel = aaDropdown.value;
        masterVolume.SetFloat("MasterVolume", volumeSlider.value);
        Settings.settings.volume = volumeSlider.value;
        QualitySettings.shadows = (ShadowQuality) shadowsDropdown.value; 
        Settings.settings.shadowsQuality = shadowsDropdown.value;
        Screen.SetResolution(resolutions[resolutionDropdown.value].width, resolutions[resolutionDropdown.value].height, fullscreenToggle.isOn);
        Settings.settings.resolutionIndex = resolutionDropdown.value;
        Settings.settings.fullscreen = fullscreenToggle.isOn;
        Settings.settings.reflectionsQuality = reflectionsDropdown.value;
        ReflectionsQuality.Set(reflectionsDropdown.value);
        
        //Controles
        for (int i = 0; i < controlsButtonsTexts.Length; i++)
            Settings.settings.controls[i] = (KeyCode) Enum.Parse(typeof(KeyCode), controlsButtonsTexts[i].text);
        Settings.settings.sensitivity[0] = sensitivity[0].value;
        Settings.settings.sensitivity[1] = sensitivity[1].value;
        Settings.settings.invertY = invert.isOn;
    }

    //Met a jour les elements du menu en fonction de Settings.settings
    private void DisplaySettings()
    {
        //Graphics
        aaDropdown.value = Settings.settings.aaLevel;
        resolutionDropdown.value = Settings.settings.resolutionIndex;
        shadowsDropdown.value = Settings.settings.shadowsQuality;
        reflectionsDropdown.value = Settings.settings.reflectionsQuality;
        fullscreenToggle.isOn = Settings.settings.fullscreen;
        volumeSlider.value = Settings.settings.volume;

        //Controles
        for (int i = 0; i < controlsButtonsTexts.Length; i++)
            controlsButtonsTexts[i].text = Settings.settings.controls[i].ToString();
        sensitivity[0].value = Settings.settings.sensitivity[0];
        sensitivity[1].value = Settings.settings.sensitivity[1];
        sensitivityTexts[0].text = Settings.settings.sensitivity[0].ToString(CultureInfo.InvariantCulture.NumberFormat);
        sensitivityTexts[1].text = Settings.settings.sensitivity[1].ToString(CultureInfo.InvariantCulture.NumberFormat);
        invert.isOn = Settings.settings.invertY;
    }
}
