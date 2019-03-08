using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Back : MonoBehaviour
{
    
    public Team team = Team.Blue;
    private bool Player_Has_Back = false;


    public void Player_Got_Back()
    {
        Player_Has_Back = true;
    }

    public void TP_Back()
    {
        if (Player_Has_Back)
        {
            Spawns.AtRandomUnused(this.gameObject);
            Player_Has_Back = false;
        }
    }
}

