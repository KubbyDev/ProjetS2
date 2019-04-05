using UnityEngine;

//Ce script gere les communication du joueur vers le jeu

public class InputManager : MonoBehaviour
{
    //0:Avancer, 1:Reculer, 2:Gauche, 3:Droite, 4:Sauter, 5:Attraper balle, 6:Jeter balle
    private KeyCode[] inputs;         //Contient toutes les touches choisies par le joueur
    private bool invertY;             //Inverse la visee en Y
    private float sensivityY ;        //Sensi horizontale
    private float sensivityX;         //Sensi verticale
    private float stopInputsTime = 0; //Le temps restant en secondes pour que les mouvements soient pris en compte

    //References a plein de scripts
    private MovementManager movement;
    private CameraManager cam;
    private PlayerInfo infos;
    private Striker striker;
    private Ninja ninja;
    private Warden warden;
    private BasicSpell basicSpell;
    private BallManager ball;
    private Back back;
    private Hook hook;

    //Reference aux menus
    private GameObject menus;
    private GameObject tabMenu;
    private GameObject pauseMenu;
    private GameObject optionsMenu;
    private GameObject classMenu;
    
    public bool inMenu = false;   //true: Empeche tout mouvement spells etc du joueur (les seuls inputs qui restent actifs sont les menus)

    void Start()
    {
        movement = GetComponent<MovementManager>();
        cam = GetComponent<CameraManager>();
        ball = GetComponent<BallManager>();
        infos = GetComponent<PlayerInfo>();
        striker = GetComponent<Striker>();
        ninja = GetComponent<Ninja>();
        warden = GetComponent<Warden>(); 
        basicSpell = GetComponent<BasicSpell>(); 
        back = GetComponent<Back>();
        hook = GetComponent<Hook>();
        menus = GameObject.Find("Menus");
        tabMenu = menus.transform.GetChild(0).gameObject;
        pauseMenu = menus.transform.GetChild(1).gameObject;
        optionsMenu = menus.transform.GetChild(2).gameObject;
        classMenu = menus.transform.GetChild(3).gameObject;
        ReloadInputs();
        
        //Bloque la souris
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    void Update()
    {
        if(!inMenu)
        {
            if (stopInputsTime <= 0)
            {
                MovementInputs();
                BallInputs();
                SpellsInputs();
            }
            
            CameraInputs();
        }

        MenusInputs();

        if (stopInputsTime > 0)                 //Met a jour le temps restant pour prendre en compte les inputs
            stopInputsTime -= Time.deltaTime;
    }

    private void SpellsInputs()
    {
        if (infos.hero == Hero.Stricker || true) //Les || true servent pour dev tranquillement, faudra les enlever
        {
            if (Input.GetKeyDown(KeyCode.R))
                striker.Speed();
            if (Input.GetKeyDown(KeyCode.E))
                striker.escape();
        }
        if (infos.hero == Hero.Warden || true)
        {
            if (Input.GetKeyDown(KeyCode.T))
                warden.Freeze();
        }
        if (infos.hero == Hero.Ninja || true)
        {
            if (Input.GetKeyDown(KeyCode.A))
                ninja.Explode_Spell();
            if (Input.GetKeyDown(KeyCode.C))
                ninja.Smoke();
        }

        //Basic Spell
        if(Input.GetKeyDown(KeyCode.G))
            basicSpell.Basic_Spell();
        
        //Power-Up
        if (Input.GetKeyDown(KeyCode.F))
            back.TP_Back();
        if (Input.GetKeyDown(KeyCode.V))
            hook.Use_Hook();
    }

    private void BallInputs()
    {
        //Balle
        //Recuperation
        if (Input.GetKeyDown(inputs[5]))
            ball.Catch();
        //Tir
        if (Input.GetKeyDown(inputs[6]))
            ball.Shoot();
    }
    
    private void MovementInputs()
    {
        //Deplacements (ZQSD)
        Vector3 move = (Input.GetKey(inputs[0]) ? 1 : 0) * transform.forward
                       + (Input.GetKey(inputs[1]) ? -1 : 0) * transform.forward
                       + (Input.GetKey(inputs[2]) ? -1 : 0) * transform.right
                       + (Input.GetKey(inputs[3]) ? 1 : 0) * transform.right;
        move.y = 0;
        movement.Move(move.normalized);

        //Sauts
        if (Input.GetKeyDown(inputs[4]))
            movement.Jump();
        
        //Dashes
        if (Input.GetKeyDown(KeyCode.LeftAlt))
            movement.Dash(move.normalized);
    }

    private void CameraInputs()
    {
        //Rotation de la camera
        Vector3 rot = new Vector3(Input.GetAxisRaw("Mouse X") * sensivityX * 2, Input.GetAxisRaw("Mouse Y") * sensivityY * 2 * (invertY ? 1 : -1), 0);
        if (rot.sqrMagnitude > 0)
            cam.Rotate(rot);
        
        //Changement de camera
        if (Input.GetKeyDown(KeyCode.F1))
            cam.changeCamera();
    }
    
    private void MenusInputs()
    {
        //Menu Tab
        if (Input.GetKeyDown(KeyCode.Tab))
            tabMenu.SetActive(true);
        if(Input.GetKeyUp(KeyCode.Tab))
            tabMenu.SetActive(false);
        
        //Menu Pause (sur Backspace au lieu de escape parce que ça fait de la merde dans l'editeur)
        if (Input.GetKeyDown(KeyCode.Backspace))
            TogglePauseMenu();
        
        //Menu de selection des classes
        //On ne peut ouvrir ce menu que si on n'est pas deja dans un autre menu
        if (Input.GetKeyDown(KeyCode.H) && (!inMenu || classMenu.activeSelf) && !GameManager.gameStarted)
            ToggleClassMenu();
        if(classMenu.activeSelf && GameManager.gameStarted)
            ToggleClassMenu();
    }

    //Va chercher les inputs dans le GameObject qui les contient
    private void ReloadInputs()
    {
        inputs = Settings.settings.controls;
        sensivityX = Settings.settings.sensitivity[0];
        sensivityY = Settings.settings.sensitivity[1];
        invertY = Settings.settings.invertY;
    }
    
    public void TogglePauseMenu()
    {
        if (pauseMenu.activeSelf || optionsMenu.activeSelf)
        //Desactivation du menu
        {
            inMenu = false;

            //Bloque la souris
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            pauseMenu.SetActive(false);
            optionsMenu.SetActive(false);
            
            ReloadInputs();
        }
        else
        //Activation du menu
        {
            inMenu = true;

            //Debloque la souris
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            pauseMenu.SetActive(true);
        }
    }

    public void ToggleClassMenu()
    {
        if (classMenu.activeSelf)
        //Desactivation du menu
        {
            inMenu = false;
            
            //Bloque la souris
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            classMenu.SetActive(false);
        }
        else
        //Activation du menu
        {
            inMenu = true;

            //Debloque la souris
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            classMenu.SetActive(true);
        }
    }

    //Arrete la prise en compte des inputs pendant duration secondes
    public void StopInputs(float duration)
    {
        stopInputsTime = duration;
    }
}