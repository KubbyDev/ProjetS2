using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public int blueScore;
    public int orangeScore;

    public GameData()
    {
        blueScore = 0;
        orangeScore = 0;
    }

    public void BlueTeamScores()
    {
        blueScore++;
    }

    public void OrangeTeamScores()
    {
        orangeScore++;
    }

}
