using UnityEngine;
using UnityEngine.UI;

public class CooldownDisplay : MonoBehaviour
{
    public static CooldownDisplay cooldownDisplayer;
    public static PlayerInfo localPlayerInfos;
        
    [SerializeField] private Image image_Spell1;
    [SerializeField] private Image image_Spell2;

    [SerializeField] private Image hider_BA;
    [SerializeField] private Image hider_Spell1;
    [SerializeField] private Image hider_Spell2;
    
    [SerializeField] private Text cd_BA;
    [SerializeField] private Text cd_Spell1;
    [SerializeField] private Text cd_Spell2;

    //Ces trois variables representent les cooldowns de ba, sp1 et sp2
    //Flemme de mettre les variables correspondantes en public dans les scripts des spells
    private float max_BA;
    private float max_Spell1;
    private float max_Spell2;
    //Ces variables servent a calculer celles qui sont au dessus
    private float previous_BA;
    private float previous_Spell1;
    private float previous_Spell2;
    
    void Awake()
    {
        cooldownDisplayer = this;
    }

    void Update()
    {
        if (localPlayerInfos == null)
            return;
        
        //Affichage du temps
        cd_BA.text = Format(localPlayerInfos.BACooldown, Settings.settings.controls[7].ToString());
        cd_Spell1.text = Format(localPlayerInfos.firstCooldown, Settings.settings.controls[8].ToString());
        cd_Spell2.text = Format(localPlayerInfos.secondCooldown, Settings.settings.controls[9].ToString());
        
        //Calcul du cooldown des spells
        if (previous_BA <= 0)
            max_BA = localPlayerInfos.BACooldown;
        previous_BA = localPlayerInfos.BACooldown;
        if (previous_Spell1 <= 0)
            max_Spell1 = localPlayerInfos.firstCooldown;
        previous_Spell1 = localPlayerInfos.firstCooldown;
        if (previous_Spell2 <= 0)
            max_Spell2 = localPlayerInfos.secondCooldown;
        previous_Spell2 = localPlayerInfos.secondCooldown;
        
        //Affichage du gris par dessus l'icone
        hider_BA.fillAmount = max_BA > 0 ? localPlayerInfos.BACooldown / max_BA : 0;
        hider_Spell1.fillAmount = max_Spell1 > 0 ? localPlayerInfos.firstCooldown / max_Spell1 : 0;
        hider_Spell2.fillAmount = max_Spell2 > 0 ? localPlayerInfos.secondCooldown / max_Spell2 : 0;
    }

    private static string Format(float time, string key)
    {
        if (time < 0.001f)
            return key;
                
        return (int) time + "." + (int) (time * 10 % 10);
    }

    //Cette fonction met a jour les images des spells (quand le joueur change de hero)
    public void UpdateSprites(Sprite[] newSprites)
    {
        image_Spell1.sprite = hider_Spell1.sprite = newSprites[0];
        image_Spell2.sprite = hider_Spell2.sprite = newSprites[1];
    }
}
