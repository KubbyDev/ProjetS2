using System.Collections;
using System.Linq;
using UnityEngine;

public partial class Skills
{
    // Actions ---------------------------------------------------------------------------------------------------------

    public void UseBasic()
    {
        if (blockInputs > 0)
            return;
        
        basic.Basic_Spell();
            
    }
    public void UseTurbo()
    {
        if (blockInputs > 0)
            return;
        
        if (infos.hero == Hero.Stricker)
            striker.Speed();
        else
            Debug.Log("UseTurbo appellee sur un " + infos.hero);
    }

    public void UseEscape()
    {
        if (blockInputs > 0)
            return;
        
        if (infos.hero == Hero.Stricker)
            striker.Escape();
        else
            Debug.Log("UseEscape appellee sur un " + infos.hero);
    }

    public void UseMagnet()
    {
        if (blockInputs > 0)
            return;
        
        if (infos.hero == Hero.Warden)
            warden.MagnetSpell();
        else
            Debug.Log("UseMagnet appellee sur un " + infos.hero);
    }

    public void UseFreeze()
    {
        if (blockInputs > 0)
            return;
        
        if (infos.hero == Hero.Warden)
            warden.Freeze();
        else
            Debug.Log("UseFreeze appellee sur un " + infos.hero);
    }

    public void UseExplode()
    {
        if (blockInputs > 0)
            return;
        
        if (infos.hero == Hero.Ninja)
            ninja.Explode_Spell();
        else
            Debug.Log("UseExplode appellee sur un " + infos.hero);
    }

    public void UseSmoke()
    {
        if (blockInputs > 0)
            return;
        
        if (infos.hero == Hero.Ninja)
            ninja.Smoke();
        else
            Debug.Log("UseSmoke appellee sur un " + infos.hero);
    }

    public void UseHook()
    {
        if (blockInputs > 0)
            return;
        
        hook.Use_Hook();
    }

    public void UseBack()
    {
        if (blockInputs > 0)
            return;
        
        back.TP_Back();
    }

    public void UsePowerShoot()
    {
        if (blockInputs > 0)
            return;
        
        powerShoot.Use_PowerShoot();
    }

    // Getters ---------------------------------------------------------------------------------------------------------

    public bool CanUseBasicAttack() => infos.BACooldown <= 0;
    public bool CanUseFirstSpell() => infos.firstCooldown <= 0;
    public bool CanUseSecondSpell() => infos.secondCooldown <= 0;
    public bool HasHook() => hook.Player_Has_Hook;
    public bool HasBack() => back.Player_Has_Back;
    public bool HasPowerShoot() => powerShoot.Player_Has_PowerShoot;
    
    public bool InRangeForFreeze() => DistanceToBall() < Freeze.bulletSpeed * Freeze.lifeTime;
    public bool InRangeForHook() => DistanceToBall() < HookBall.Speed * Hook.lifeTime;
    
    public bool CanUseHook() => HasHook() && InRangeForHook();
    public bool CanUseMagnet() => CanUseSecondSpell() && DistanceToBall() < infos.maxCatchRange + Warden.MagnetBonusRange;
    public bool CanUseFreeze() => CanUseFirstSpell() && InRangeForFreeze();

    /// <summary> Renvoie le spawner le plus proche parmi ceux du type demande. </summary>
    /// <summary> availableOnly = True => Ne renvera le spawner que si son PU est disponible </summary>
    /// <param name="type"></param>
    /// <param name="availableOnly"></param>
    /// <returns></returns>
    public GameObject GetNearestPowerUp(PowerUp type, bool availableOnly = true)
    {
        GameObject[] spawners = GameObject.FindGameObjectsWithTag("Spawner") //Tous les spawners
            .Where(spawner => spawner.GetComponent<Trigger>().type == type)  //Les spawners qui donnent le bon type de PU
            .Where(spawner => spawner.GetComponent<Trigger>().timer < 0)     //Les spawners ont deja fait pop leur PU
            .ToArray();

        float minDistance = float.PositiveInfinity;
        GameObject nearest = null;

        foreach (GameObject spawner in spawners)
        {
            float dist = Vector3.Distance(spawner.transform.position, transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                nearest = spawner;
            }
        }

        return nearest;
    }
    

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

    public void UseEscapeSmartly(Vector3 Destination)
    {
        LookAt(Destination);

        if (!usingEscape)
            StartCoroutine(EscapeCoroutine());
        
    }

    private bool usingEscape;
    IEnumerator EscapeCoroutine()
     {
         usingEscape = true;
         yield return new WaitForSeconds(0.4f);
         
         UseEscape();
         usingEscape = false;
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

     public void UseBasicSmartly(GameObject target)
     {
         //Calcul de l'orientation cible
         LookAt(PredictContactPoint(target, target.GetComponent<PlayerInfo>().velocity, BasicSpellBall.BulletSpeed));

         if (!usingBasic)
             StartCoroutine(BasicCoroutine());
     }

     private bool usingBasic;
     IEnumerator BasicCoroutine()
     {
         usingBasic = true;

         yield return new WaitForSeconds(0.4f);

         UseBasic();
         usingBasic= false;
     }
}