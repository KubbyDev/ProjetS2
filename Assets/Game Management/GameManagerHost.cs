using UnityEngine;

public class GameManagerHost : MonoBehaviour
{
    public static GameManagerHost script; //Reference a ce script, visible partout

    private void Awake()
    {
        script = this;
        GameManager.gameConfig = GamePreset.Classic.Config();
    }

    //Le host recoit l'event de but et informe tous les clients
    public static void OnGoal(bool isBlue)
    {
        GameDataSync.SendOnGoalData(isBlue);
    }
} 