using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;

public class OptionsMenu : MonoBehaviour
{
    //Cette classe sert a enregistrer les settings dans un fichier
    //pour y acceder au prochain lancement du jeu
    public class GameSettings
    {
        public bool fullscreen = true;
        public int aaLevel = 3;
        public int resolutionIndex = 0;
        public int shadowsQuality = 3;
        public float volume = 0.5f;

        //Controles                   0:Avancer  1:Reculer  2:Gauche   3:Droite   4:Sauter       5:Attraper ball 6:Jeter ball
        public KeyCode[] controls = { KeyCode.Z, KeyCode.S, KeyCode.Q, KeyCode.D, KeyCode.Space, KeyCode.Mouse0, KeyCode.Mouse1 };
    }

    [SerializeField] private Text[] controlsButtonsTexts;   //References aux textes des boutons des controls (les index sont les memes que pour les cles dans GameSettings)
    [SerializeField] private GameObject graphicsMenu;       //Reference a l'onglet Graphics
    [SerializeField] private GameObject controlsMenu;       //Reference a l'onglet Controls
    [SerializeField] private Button graphicsButton;         //Reference au bouton de l'onglet Graphics
    [SerializeField] private Button controlsButton;         //Reference au bouton de l'onglet Controls

    [SerializeField] private AudioSource audioSource;       //Reference au systeme de gestion du son        //TODO
    [SerializeField] private Dropdown resolutionDropdown;   //Reference au dropdown des resolutions
    [SerializeField] private Dropdown shadowsDropdown;      //Reference au dropdown des ombres
    [SerializeField] private Dropdown aaDropdown;           //Reference au dropdown de l'anti aliasing
    [SerializeField] private Toggle fullscreenToggle;       //Reference au toggle du fullscreen
    [SerializeField] private Slider volumeSlider;           //Reference au slider du volume

    private GameSettings settings;        //Liste des settings
    private Resolution[] resolutions;     //Liste des resolutions que l'ecran peut afficher
    private string settingsFilePath;      //Chemin d'acces du fichier dans lequel les settings sont enregistres
    private int currentKey = -1;          //L'index de la touche que l'utilisateur est en train d'assigner

    //  Initialisation  --------------------------------------------------------------------------------------------------------

    void OnEnable()
    {
        RefreshSettings();
    }

    public void RefreshSettings()
    {
        settingsFilePath = Application.persistentDataPath + "/settings.json";

        //On rempli le dropdown des resolutions avec toutes les resolutions que l'ecran peut afficher
        resolutions = Screen.resolutions;
        foreach (Resolution res in Screen.resolutions)
            resolutionDropdown.options.Add(new Dropdown.OptionData(res.ToString()));

        //Si le fichier de sauvegarde des settings n'existe pas encore, on le cree
        if (!File.Exists(settingsFilePath))
        {
            settings = new GameSettings();
            SaveSettings();
        }

        //Les settings sont lues dans le fichier et sont charges
        LoadSettings();
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
        settings.fullscreen = value;
    }

    //Dropdown de la resolution
    public void OnResolutionChange(int index)
    {
        settings.resolutionIndex = index;
    }

    //Dropdown des ombres
    public void OnShadowsChange(int index)
    {
        settings.shadowsQuality = index;
    }

    //Dropdown de l'AA
    public void OnAntialiasingChange(int index)
    {
        settings.aaLevel = index;
    }

    //Slider du volume
    public void OnVolumeChange(float volume)
    {
        settings.volume = volume;
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
                settings.controls[currentKey] = key;
                controlsButtonsTexts[currentKey].text = key.ToString();

                currentKey = -1;
            }
        }
    }

    void OnDisable()
    {
        currentKey = -1;
    }

    //  Enregistrement, chargement et lecture des settings  --------------------------------------------------------------------

    //Bouton Apply
    public void OnApplySettings()
    {
        //Applique les settings dans le jeu
        ApplySettings();

        //Enregistre les settings dans le fichier
        SaveSettings();
    }

    //Enregistre les settings dans le fichier de sauvegarde
    private void SaveSettings()
    {
        string jsonData = JsonUtility.ToJson(settings, true);
        File.WriteAllText(settingsFilePath, jsonData);
    }

    //Va chercher les settings dans le fichier de sauvegarde
    private void LoadSettings()
    {
        settings = JsonUtility.FromJson<GameSettings>(File.ReadAllText(settingsFilePath));
        ApplySettings();
    }

    //Applique les settings sur le jeu
    private void ApplySettings()
    {
        //Graphics

        //Mise a jour des options dans Unity
        QualitySettings.antiAliasing = (int) Mathf.Pow(2, settings.aaLevel);
        //audioSource.volume = settings.volume;
        QualitySettings.shadows = (ShadowQuality) settings.shadowsQuality;
        Screen.fullScreen = settings.fullscreen;
        Screen.SetResolution(resolutions[settings.resolutionIndex].width, resolutions[settings.resolutionIndex].height, Screen.fullScreen);

        //Mise a jour de l'affichage
        aaDropdown.value = settings.aaLevel;
        resolutionDropdown.value = settings.resolutionIndex;
        shadowsDropdown.value = settings.shadowsQuality;
        fullscreenToggle.isOn = settings.fullscreen;
        volumeSlider.value = settings.volume;

        //Controls
        //Mise a jour des options dans Unity
        GameObject.Find("Inputs").GetComponent<Inputs>().inputs = settings.controls;
        
        //Mise a jour de l'affichage
        for (int i = 0; i < controlsButtonsTexts.Length; i++)
            controlsButtonsTexts[i].text = settings.controls[i].ToString();
    }
}
