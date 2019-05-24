using System;
using System.IO;
using System.Reflection;
using UnityEngine;

//Cette classe gere les informations a enregistrer sur le disque dur
//Tous les fields publics et non static seront enregistres

public class Settings
{
    //Pour acceder aux enregistrement, attendre d'avoir eu un Settings.Load() (fait dans MainMenu.Start())
    //Puis faire Settings.settings.<nom de la variable>
    public static Settings settings;  

    //Chemin d'acces du fichier dans lequel les settings sont enregistres
    private static string settingsFilePath = Application.persistentDataPath + "/settings.json";
    
    //Variables a enregistrer ------------------------------------------------------------------------------------------
    
    //Graphismes
    public bool fullscreen = true;
    public int aaLevel = 3;
    public int resolutionIndex = 0;
    public int shadowsQuality = 3;
    public int reflectionsQuality = 2;
    
    //Son
    public float volume = 0.5f;

    //Controles (les controles definis ici sont les controles par defaut)
    public KeyCode[] controls =
    {
        KeyCode.Z,         //0: Avancer
        KeyCode.S,         //1: Reculer
        KeyCode.Q,         //2: Gauche
        KeyCode.D,         //3: Droite
        KeyCode.Space,     //4: Sauter
        KeyCode.Mouse0,    //5: Attraper balle
        KeyCode.Mouse1,    //6: Jeter balle
        KeyCode.F,         //7: Basic Attack
        KeyCode.A,         //8: 1st Spell
        KeyCode.E,         //9: 2nd Spell
        KeyCode.Alpha1,    //10: 1st powerup: back
        KeyCode.Alpha2,    //11: 2nd powerup: hook
        KeyCode.Alpha3,    //12: 3rd powerup: power
        KeyCode.F1,        //13: Change cam
        KeyCode.Tab,       //14: Player List
        KeyCode.H,         //15: Change Hero
        KeyCode.Backspace, //16: Pause Menu
    };
    public float[] sensitivity = {0.5f,0.5f};
    public bool invertY = false;

    //Autres
    public Hero defaultHero = Hero.Stricker;
    public string nickname = "";
    
    //------------------------------------------------------------------------------------------------------------------
    
    //Enregistre les variables sur le disque
    public static void Save()
    {
        File.WriteAllText(settingsFilePath, JsonUtility.ToJson(settings, true));
    }

    //Charge les variables depuis l'enregistrement
    public static void Load()
    {
        //Si le fichier de sauvegarde des settings n'existe pas encore, on le cree
        //Les valeurs de settings seront donc les valeurs par defaut
        if (!File.Exists(settingsFilePath))
            CreateSettingsFile();
        else
        {
            try
            {
                Settings file = JsonUtility.FromJson<Settings>(File.ReadAllText(settingsFilePath));

                //Si les donnees sont correctes on les utilise, sinon on prend les settings par defaut
                if (file.sensitivity.Length == 2 && file.controls.Length == new Settings().controls.Length)
                    settings = file;
                else
                    CreateSettingsFile();
            }
            //Si le fichier est incorrect on prend les settings par defaut
            catch(Exception) { CreateSettingsFile(); }
        }
    }

    private static void CreateSettingsFile()
    {
        settings = new Settings();
        Save();
    }
}