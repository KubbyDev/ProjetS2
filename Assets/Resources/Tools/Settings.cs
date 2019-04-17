﻿using System.IO;
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
    public float volume = 0.5f;

    //Controles                   0:Avancer  1:Reculer  2:Gauche   3:Droite   4:Sauter       5:Attraper ball 6:Jeter ball
    public KeyCode[] controls = { KeyCode.Z, KeyCode.S, KeyCode.Q, KeyCode.D, KeyCode.Space, KeyCode.Mouse0, KeyCode.Mouse1 };
    public float[] sensitivity = {1f,1f};
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
        if (!File.Exists(settingsFilePath))
        {
            settings = new Settings();
            Save();
        }
        else
            settings = JsonUtility.FromJson<Settings>(File.ReadAllText(settingsFilePath));
    }
}