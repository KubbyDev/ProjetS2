using UnityEngine;
using UnityEngine.UI;

//Ce script gere le menu de selection des classes 

public class ClassMenu : MonoBehaviour
{
    private PlayerInfo infos;             //Reference au script qui contient toutes les infos du joueur
    private InputManager inputManager;    //Reference au script qui gere les inputs
    private RawImage image;               //L'image qui affiche le hero selectionne
    private GameObject strickerInfo;      //Les informations sur le stricker
    private GameObject wardenInfo;        //Les informations sur le warden
    private GameObject ninjaInfo;         //Les informations sur le ninja
    private bool init = false;            //Permet de savoir si les infos ont ete initialisee (les infos venant du playerinfo)
    
    [SerializeField] private bool inMainMenu;       //True: Ce menu est ouvert depuis le menu pricipal, pas depuis une partie
    [SerializeField] private Texture strickerImage; //L'image qui s'affiche quand on le selectionne
    [SerializeField] private Texture wardenImage;   //L'image qui s'affiche quand on le selectionne
    [SerializeField] private Texture ninjaImage;    //L'image qui s'affiche quand on le selectionne
    
    void Start()
    {
        //Recuperation des zones de textes pour chaque classe, et de l'image
        //Les 3 zones de textes sont fixes, et sont juste masquees
        //Mais il n'y a qu'une seule image, qui est modifiee quand on change de hero
        Transform infoBackground = transform.Find("InfoBackground");
        strickerInfo = infoBackground.Find("StrickerInfo").gameObject;
        wardenInfo = infoBackground.Find("WardenInfo").gameObject;
        ninjaInfo = infoBackground.Find("NinjaInfo").gameObject;
        image = transform.Find("Image").GetComponent<RawImage>();
        
        UpdateDisplay(Settings.settings.defaultHero);
    }

    void Update()
    {
        if (!inMainMenu && !init && PlayerInfo.localPlayer != null)
        {
            //Recuperation des references aux scripts
            //Si on est en jeu (pas dans le menu principal)
            infos = PlayerInfo.localPlayer.GetComponent<PlayerInfo>();
            inputManager = PlayerInfo.localPlayer.GetComponent<InputManager>();

            init = true;
        }
    }

    //Methode appellee quand on clique sur le bouton Stricker
    public void OnStrikerButtonCliked()
    {
        UpdateDisplay(Hero.Stricker);
        
        //Si on est en jeu (pas dans le menu principal)
        if (!inMainMenu)
        {
            infos.SetHero(Hero.Stricker);
            infos.UpdateInfos();
            inputManager.ToggleClassMenu();
        }

        //Settings.settings contient toutes les variables enregistrees
        Settings.settings.defaultHero = Hero.Stricker;
        Settings.Save();
    }

    //Methode appellee quand on clique sur le bouton Warden
    public void OnWardenButtonCliked()
    {
        UpdateDisplay(Hero.Warden);
        
        //Si on est en jeu (pas dans le menu principal)
        if (!inMainMenu)
        {
            infos.SetHero(Hero.Warden);
            infos.UpdateInfos();
            inputManager.ToggleClassMenu();
        }
        
        //Settings.settings contient toutes les variables enregistrees
        Settings.settings.defaultHero = Hero.Warden;
        Settings.Save();
    }

    //Methode appellee quand on clique sur le bouton Ninja
    public void OnNinjaButtonCliked()
    {
        UpdateDisplay(Hero.Ninja);
        
        //Si on est en jeu (pas dans le menu principal)
        if (!inMainMenu)
        {
            infos.SetHero(Hero.Ninja);
            infos.UpdateInfos();
            inputManager.ToggleClassMenu();
        }
        
        //Settings.settings contient toutes les variables enregistrees
        Settings.settings.defaultHero = Hero.Ninja;
        Settings.Save();
    }

    public void UpdateDisplay(Hero selected)
    {
        //On change le texte
        strickerInfo.SetActive(selected == Hero.Stricker);
        wardenInfo.SetActive(selected == Hero.Warden);
        ninjaInfo.SetActive(selected == Hero.Ninja);

        //On change l'image
        image.texture = selected == Hero.Stricker
            ? strickerImage
            : selected == Hero.Warden ? wardenImage : ninjaImage;
    }
    
    public void OnResumeClicked()
    {
        //Desactive le menu (uniquement en jeu, dans le menu principal le bouton back amene sur MainMenu.OnBackClicked())
        inputManager.ToggleClassMenu();
    }
}
