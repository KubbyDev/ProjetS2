using System;
using System.Collections;
using System.Linq;
using UnityEngine;

//Cette classe permet de creer un objet contenant des informations sur la partie
//Nombre de buts max, temps max, nombre de joueurs par Team etc

public class GameConfig
{
    public readonly int maxGoals;         //Nombre max de buts: 0 = infini
    public readonly int playersPerTeam;   //Nombre de joueurs par equipe
    public readonly float gameDuration;   //Duree de la partie

    //Constructeur
    public GameConfig(int pMaxGoals, float pGameDuration, int pPlayersPerTeam)
    {
        maxGoals = pMaxGoals;
        gameDuration = pGameDuration;
        playersPerTeam = pPlayersPerTeam;
    }
}

//Pour utiliser un preset on peut faire
//GameConfig config = GamePreset.Classic.Config();
public enum GamePreset
{
    Classic = 0  //maxGoals = infini, gameDuration = 5min, playersPerTeam = 3
}

internal static class ConfigMethods
{
    //Renvoie une Config depuis un Preset
    public static GameConfig Config(this GamePreset name)
    {
        switch (name)
        {
            case GamePreset.Classic:
                GameConfig config = new GameConfig(0, 5 * 60, 3);
                return config;
            
            default:
                Debug.Log("Ce preset n'existe pas " + name);
                return null;
        }
    }

    //Renvoie une Config depuis une Hashtable
    //Quand la salle est creee, elle integre une Hashtable CustomProperties
    //Cette methode sert a creer une config a partir des CustomProperties
    public static GameConfig Config(this ExitGames.Client.Photon.Hashtable config)
    {
        return new GameConfig((int) config["g"], (int) config["d"], (int) config["p"]);
    }
}