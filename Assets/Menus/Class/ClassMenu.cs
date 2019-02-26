using UnityEngine;

public class ClassMenu : MonoBehaviour
{
    private PlayerInfo infos;
    private InputManager inputManager;
    
    private void Start()
    {
        infos = PlayerInfo.localPlayer.GetComponent<PlayerInfo>();
        inputManager = PlayerInfo.localPlayer.GetComponent<InputManager>();
    }

    public void OnStrikerButtonCliked()
    {
        infos.hero = Hero.Stricker;
        inputManager.ToggleClassMenu();
    }

    public void OnWardenButtonCliked()
    {
        infos.hero = Hero.Warden;
        inputManager.ToggleClassMenu();
    }

    public void OnNinjaButtonCliked()
    {
        infos.hero = Hero.Ninja;
        inputManager.ToggleClassMenu();
    }
}
