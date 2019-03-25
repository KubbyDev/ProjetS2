using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : MonoBehaviour
{
    [SerializeField] private float MagnetSpellDuration = 5f;                // Duree du bonus de range
    [SerializeField] private float MagnetCooldown = 20f;                    // Cooldown du Magnet
    [SerializeField] private float MagnetBonusRange = 4f;                   // Bonus de range
    [SerializeField] private bool MagnetOffCooldown = true;                 // Indicateur en cooldown

    private PlayerInfo Info;                                                // Informations du joueur

    void Start()                                                            
    {
        Info = GetComponent<PlayerInfo>();                                  // Recuperation des informations du joueur
    }
    
    public void MagnetSpell()
    {
        StartCoroutine(MagnetCoroutine());
    }

    IEnumerator MagnetCoroutine()
    {
        if(MagnetOffCooldown)                                               // Si le spell n'est pas en cooldown
        {
            Info.maxcatchrange += MagnetBonusRange;                         // Application du bonus de range
            MagnetOffCooldown = false;                                      // Le spell passe en cooldown
            yield return new WaitForSeconds(MagnetSpellDuration);           // Duree du bonus
            Info.maxcatchrange -= MagnetBonusRange;                         // Retour a la normale de la range
            yield return new WaitForSeconds(MagnetCooldown);                // Duree du cooldown
            MagnetOffCooldown = true;                                       // Le spell redevient utilisable
        }
    }
}
