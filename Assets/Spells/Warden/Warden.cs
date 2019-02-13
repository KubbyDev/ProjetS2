using System.Collections;
using UnityEngine;

public class Warden : MonoBehaviour
{
    [SerializeField] private float Freeze1_Duration = 1f;  //Duree du freeze complet (pas de recuperation, pas de mouvement)
    [SerializeField] private float Freeze2_Duration = 4f;  //Duree du freeze partiel (pas de mouvement mais recuperation possible)
    [SerializeField] private float Freeze_Cooldown = 20f;  //Duree du cooldown

    private GameObject ball;                   //Reference a la balle
    private Ball ballScript;                   //Reference au script de la balle
    private bool Freeze_Off_Cooldown = true;   //Vrai si le cooldown du freeze est termine

    public void Freeze_Spell()
    {
        StartCoroutine(Freeze_Coroutine());
    }

    IEnumerator Freeze_Coroutine()
    {
        if(Freeze_Off_Cooldown)
        {
            ballScript.FreezeBall();
            yield return new WaitForSeconds(Freeze1_Duration);
            ballScript.DeFreezeBall();
            // Freeze Part 2 Diable Gravity unless ball caught or time up
        }
    }
}
