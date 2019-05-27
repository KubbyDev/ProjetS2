﻿using System.Collections;
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
    public bool InRangeForFreeze() => DistanceToBall() < Freeze.bulletSpeed * Freeze.lifeTime;
    public bool InRangeForHook() => DistanceToBall() < HookBall.Speed * Hook.lifeTime;

    // Utilisation intelligente des spells  ----------------------------------------------------------------------------

    /// <summary>
    /// Utilise explode et fonce derriere le joueur target
    /// </summary>
    /// <param name="target"></param>
    public void UseExplodeSmartly(GameObject target)
    {
        UseExplode();

        MoveTo(target.transform.position + 2.0f * (EnemyGoal().transform.position - target.transform.position).normalized, true, true, 0.1f);
    }
    
    public void UseFreezeSmartly()
    {
        //Calcul de l'orientation cible
        LookAt(PredictContactPoint(Ball.ball, Ball.rigidBody.velocity, Freeze.bulletSpeed));

        if (!usingFreeze)
            StartCoroutine(FreezeCoroutine());
    }

    private bool usingFreeze;
    IEnumerator FreezeCoroutine()
    {
        usingFreeze = true;

        yield return new WaitForSeconds(0.4f);

        UseFreeze();
        usingFreeze = false;
    }
    
    public void UseHookSmartly()
    {
        //Calcul de l'orientation cible
        LookAt(PredictContactPoint(Ball.ball, Ball.rigidBody.velocity, HookBall.Speed));

        if (!usingHook)
            StartCoroutine(HookCoroutine());
    }

    private bool usingHook;
    IEnumerator HookCoroutine()
    {
        usingHook = true;

        yield return new WaitForSeconds(0.4f);

        UseHook();
        usingHook = false;
    }
}