using UnityEngine;
using UnityEngine.UI;

public class PowerUpDisplay : MonoBehaviour
{
    [SerializeField] private GameObject hook;
    [SerializeField] private GameObject back;
    [SerializeField] private GameObject power;

    private Text hookText;
    private Text backText;
    private Text powerText;

    private Hook hookScript;
    private Back backScript;
    private PowerShoot powerScript;

    private bool init = false; //Passe a true quand l'initialisation s'est faite

    private void Init()
    {
        init = true;
        
        hookScript = PlayerInfo.localPlayer.GetComponent<Hook>();
        backScript = PlayerInfo.localPlayer.GetComponent<Back>();
        powerScript = PlayerInfo.localPlayer.GetComponent<PowerShoot>();

        hookText = hook.transform.Find("Text").GetComponent<Text>();
        backText = back.transform.Find("Text").GetComponent<Text>();
        powerText = power.transform.Find("Text").GetComponent<Text>();
    }
    
    void Update()
    {
        //On initialise si le PlayerInfo est pret
        if (!init && PlayerInfo.localPlayer != null)    
            Init();

        //On ne fait rien tant que l'init n'est pas faite
        if (!init)
            return;
        
        //Active les affichages pour les powerups que le joueur a
        hook.SetActive(hookScript.Player_Has_Hook);
        back.SetActive(backScript.Player_Has_Back);
        power.SetActive(powerScript.Player_Has_PowerShoot);
        
        //Met a jour le texte en dessous (touche pour activer le power up)
        backText.text = Settings.settings.controls[10].ToString();
        hookText.text = Settings.settings.controls[11].ToString();
        powerText.text = Settings.settings.controls[12].ToString();
    }
}
