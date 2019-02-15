using UnityEngine;
using UnityEngine.UI;
using System.IO;

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
    }

    [SerializeField] private AudioSource audioSource;       //Reference au systeme de gestion du son                                    //TODO
    [SerializeField] private Dropdown resolutionDropdown;   //Reference au dropdown des resolutions

    private GameSettings settings;        //Liste des settings
    private Resolution[] resolutions;     //Liste des resolutions que l'ecran peut afficher
    private string settingsFilePath;      //Chemin d'acces du fichier dans lequel les settings sont enregistres

    void OnEnable()
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
        QualitySettings.antiAliasing = (int)Mathf.Pow(2, settings.aaLevel);
        //audioSource.volume = settings.volume;
        QualitySettings.shadows = (ShadowQuality) settings.shadowsQuality;
        Screen.fullScreen = settings.fullscreen;
        Screen.SetResolution(resolutions[settings.resolutionIndex].width, resolutions[settings.resolutionIndex].height, Screen.fullScreen);

        //TODO Mettre a jour l'affichage
    }
}
