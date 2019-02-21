using System;
using UnityEngine;

public class GameConfig
{
    public int maxGoals;        //Le nombre de buts pour gagner
    public float gameDuration;  //Le temps de jeu maximal en secondes
    public int playersPerTeam;  //Le nombre de joueurs par equipe

    public GameConfig(int pMaxGoals, float pGameDuration, int pPlayersPerTeam)
    {
        maxGoals = pMaxGoals;
        gameDuration = pGameDuration;
        playersPerTeam = pPlayersPerTeam;
    }
    
    //Renvoie un preset
    public static GameConfig Preset(string name)
    {
        switch (name)
        {
            case "Classic": return new GameConfig(Int32.MaxValue, 5 * 60, 4);
            
            default:
                Debug.Log("Ce preset n'existe pas " + name);
                return null;
        }
    }
}
