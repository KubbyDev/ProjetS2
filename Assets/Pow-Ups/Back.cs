using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Back : MonoBehaviour
{
    public Team team = Team.Blue;
    private Transform blue_pos;
    private Transform orange_pos;

    private bool Player_Has_Back = true;

    public void Start()
    {
        if (Player_Has_Back)
        {
            blue_pos = GameObject.Find("blue_pos").transform;
            orange_pos = GameObject.Find("orange_pos").transform;
            TP_Back();
            Player_Has_Back = false;
        }
    }

    public void Player_Got_Back()
    {
        Player_Has_Back = true;
    }
    public void TP_Back()
    {
        if (team == Team.Blue)
            transform.SetPositionAndRotation(blue_pos.position, blue_pos.rotation);
        else
            transform.SetPositionAndRotation(orange_pos.position, orange_pos.rotation);
    }
}
