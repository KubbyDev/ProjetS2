using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CooldownDisplay : MonoBehaviour
{
    public static PlayerInfo localPlayerInfos;
    
    [SerializeField] private Image image_Spell1;
    [SerializeField] private Image image_Spell2;
    
    [SerializeField] private Text cd_BA;
    [SerializeField] private Text cd_Spell1;
    [SerializeField] private Text cd_Spell2;

    void Update()
    {
        if (localPlayerInfos == null)
            return;
        
        cd_BA.text = Format(localPlayerInfos.BACooldown);
        cd_Spell1.text = Format(localPlayerInfos.firstCooldown);
        cd_Spell2.text = Format(localPlayerInfos.secondCooldown);
    }

    private static string Format(float time)
    {
        return (int) time + "." + (int) (time * 10 % 10);
    }
}
