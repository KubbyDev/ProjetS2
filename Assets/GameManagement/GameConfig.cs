using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class GameConfig
{
    public object[] parameters =
    {
        0,              //MaxGoals
        5.0f * 60.0f,   //GameDuration
        4               //PlayersPerTeam
    };

    public GamePreset preset;
    
    public enum Parameters
    {
        MaxGoals = 0,
        GameDuration = 1,
        PlayersPerTeam = 2
    }

    public GameConfig(int maxGoals, float gameDuration, int playersPerTeam)
    {
        parameters[(int) Parameters.MaxGoals] = maxGoals;
        parameters[(int) Parameters.GameDuration] = gameDuration;
        parameters[(int) Parameters.PlayersPerTeam] = playersPerTeam;
    }
}

public enum GamePreset
{
    Classic = 0
}

static class ConfigMethods
{
    //Renvoie un preset
    public static GameConfig Config(this GamePreset name)
    {
        switch (name)
        {
            case GamePreset.Classic:
                GameConfig config = new GameConfig(Int32.MaxValue, 5 * 60, 4);
                config.preset = GamePreset.Classic;
                return config;
            
            default:
                Debug.Log("Ce preset n'existe pas " + name);
                return null;
        }
    }

    public static GameConfig Config(this ExitGames.Client.Photon.Hashtable config)
    {
        return new GameConfig((int) config["g"], (float) (int) config["d"], (int) config["p"]);
    }
}