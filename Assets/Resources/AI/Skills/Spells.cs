using UnityEngine;

public partial class Skills
{
    // Actions ---------------------------------------------------------------------------------------------------------
    
    public void UseTurbo()
    {
        if(infos.hero == Hero.Stricker)
            striker.Speed();
        else
            Debug.Log("UseTurbo appellee sur un " + infos.hero);
    }
    
    public void UseEscape()
    {
        if(infos.hero == Hero.Stricker)
            striker.Escape();
        else
            Debug.Log("UseEscape appellee sur un " + infos.hero);
    }
    
    public void UseMagnet()
    {
        if(infos.hero == Hero.Warden)
            warden.MagnetSpell();
        else
            Debug.Log("UseMagnet appellee sur un " + infos.hero);
    }
    
    public void UseFreeze()
    {
        if(infos.hero == Hero.Warden)
            warden.Freeze();
        else
            Debug.Log("UseFreeze appellee sur un " + infos.hero);
    }

    public void UseExplode()
    {
        if(infos.hero == Hero.Ninja)
            ninja.Explode_Spell();
        else
            Debug.Log("UseExplode appellee sur un " + infos.hero);
    }

    public void UseSmoke()
    {
        if(infos.hero == Hero.Ninja)
            ninja.Smoke();
        else
            Debug.Log("UseSmoke appellee sur un " + infos.hero);
    }

    public void UseHook()
    {
        hook.Use_Hook();
    }

    public void UseBack()
    {
        back.TP_Back();
    }

    public void UsePowerShoot()
    {
        powerShoot.Use_PowerShoot();
    }
    
    // Getters ---------------------------------------------------------------------------------------------------------

    public bool CanUseBasicAttack () => infos.BACooldown <= 0;
    public bool CanUseFirstSpell () => infos.firstCooldown <= 0;
    public bool CanUseSecondSpell () => infos.secondCooldown <= 0;
    public bool HasHook () => hook.Player_Has_Hook;
    public bool HasBack () => back.Player_Has_Back;
    public bool HasPowerShoot () => powerShoot.Player_Has_PowerShoot;
}