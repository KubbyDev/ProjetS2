using UnityEngine;

public class GameData
{
    public static bool receivedFirstPacket; //True: on a recu les infos de base et on peut spawn le joueur
    
    public static bool isGameStarted;       //True: on est en game, pas en pregame
    
    //Pregame variables
    public static float timeLeftToStart;    //Le temps avant le debut de la partie
    
    //Game variables
    public static float timeLeft;           //Le temps avant la fin de la partie

    public static void ReceiveFirstPacket(float time, GamePreset preset, int spawnSeed)
    {
        timeLeftToStart = time;
        GameManager.gameConfig = preset.Config();
        Spawns.randomSeed = spawnSeed;
    }

    public static object[] SendFirstPacket()
    {
        return new object[]{timeLeftToStart, (int) GameManager.gameConfig.preset, Spawns.randomSeed};
    }

    public static void ReceiveOnGoalData()
    {
        
    }

    public static object[] SendOnGoalData(bool isBlue)
    {
        return new object[]{isBlue, Ball.ball.transform.position};
    }
}
