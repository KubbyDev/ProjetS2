using System.Collections;
using UnityEngine;

public class Warden : MonoBehaviour
{
    [SerializeField] private float Freeze1_Duration = 1f;  //Duree du freeze complet (pas de recuperation, pas de mouvement)
    
    //J'ai mis ces variables en commentaire parce qu'elle etaient pas utilisees
    //SerializeField] private float Freeze2_Duration = 4f;  //Duree du freeze partiel (pas de mouvement mais recuperation possible)
    //[SerializeField] private float Freeze_Cooldown = 20f;  //Duree du cooldown

    private bool Freeze_Off_Cooldown = true;   //Vrai si le cooldown du freeze est termine

    public void Freeze_Spell()
    {
        StartCoroutine(Freeze_Coroutine());
    }

    IEnumerator Freeze_Coroutine()
    {
        if(Freeze_Off_Cooldown)
        {
            Ball.script.FreezeBall();
            yield return new WaitForSeconds(Freeze1_Duration);
            Ball.script.DeFreezeBall();
            // Freeze Part 2 Diable Gravity unless ball caught or time up
        }
    }
}
