using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public int goalsScored;
    public Team team;

    public enum Team
    {
        Blue = 1,
        Orange = -1
    }
}
