using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private bool invertY = false;                   //Inverse la visee en Y
    [SerializeField] [Range(0, 10)] private float sensivityY = 2;    //Sensi horizontale
    [SerializeField] [Range(0, 10)] private float sensivityX = 2;    //Sensi verticale

    //0:Avancer, 1:Reculer, 2:Gauche, 3:Droite, 4:Sauter, 5:Attraper balle, 6:Jeter balle
    private KeyCode[] inputs;         //Contient toutes les touches choisies par le joueur
    private float stopInputsTime = 0; //Le temps restant en secondes pour que les inputs soient pris en compte

    //References a plein de scripts
    private MovementManager movement;
    private CameraManager cam;
    private Striker striker;
    private Ninja ninja;
    private BallManager ball;
    private Back back;

    //Reference au GameObject qui contient tous les menus
    private GameObject menus;
    private GameObject tabMenu;
    private GameObject pauseMenu;
    private GameObject optionsMenu;

    public bool canMove = true;     //true: Empeche tout mouvement spells etc du joueur (les seuls inputs qui restent actifs sont les menus)

    void Start()
    {
        movement = GetComponent<MovementManager>();
        cam = GetComponent<CameraManager>();
        ball = GetComponent<BallManager>();
        striker = GetComponent<Striker>();
        ninja = GetComponent<Ninja>();
        back = GetComponent<Back>();
        menus = GameObject.Find("Menus");
        tabMenu = menus.transform.GetChild(0).gameObject;
        pauseMenu = menus.transform.GetChild(1).gameObject;
        optionsMenu = menus.transform.GetChild(2).gameObject;
        pauseMenu.GetComponent<PauseMenu>().SetInputManager(this);
        ReloadInputs();
        
        //Bloque la souris
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    void Update()
    {
        if(canMove && stopInputsTime <= 0)
        {
            //Deplacements (ZQSD)
            Vector3 move = (Input.GetKey(inputs[0]) ? 1 : 0) * transform.forward
                           + (Input.GetKey(inputs[1]) ? -1 : 0) * transform.forward
                           + (Input.GetKey(inputs[2]) ? -1 : 0) * transform.right
                           + (Input.GetKey(inputs[3]) ? 1 : 0) * transform.right;
            move.y = 0;
            movement.Move(move.normalized);

            //Rotation de la camera
            Vector3 rot = new Vector3(Input.GetAxisRaw("Mouse X") * sensivityX, Input.GetAxisRaw("Mouse Y") * sensivityY * (invertY ? 1 : -1), 0);
            if (rot.sqrMagnitude > 0)
                cam.Rotate(rot);

            //Sauts
            if (Input.GetKeyDown(inputs[4]))
                movement.Jump(move);       //Cette fonction prend en parametre les inputs ZQSD pour les dashes

            //Spells
            if (Input.GetKeyDown(KeyCode.A))
                ninja.Explode_Spell();
            if (Input.GetKeyDown(KeyCode.R))
                striker.Speed();
            if (Input.GetKeyDown(KeyCode.E))
                striker.escape();

            //Power-Up
            if (Input.GetKeyDown(KeyCode.F))
                back.TP_Back();

            //Changement de camera
            if (Input.GetKeyDown(KeyCode.F1))
                cam.changeCamera();

            //Balle
            //Recuperation
            if (Input.GetKeyDown(inputs[5]))
                ball.Catch();
            //Tir
            if (Input.GetKeyDown(inputs[6]))
                ball.Shoot();
        }

        //Menu Tab
        if (Input.GetKeyDown(KeyCode.Tab))
            tabMenu.SetActive(!tabMenu.activeSelf);
        //Menu Pause (sur Backspace au lieu de escape parce que ça fait de la merde dans l'editeur)
        if (Input.GetKeyDown(KeyCode.Backspace))
            TogglePauseMenu();

        if (stopInputsTime > 0)                 //Met a jour le temps restant pour prendre en compte les inputs
            stopInputsTime -= Time.deltaTime;
    }

    //Va chercher les inputs dans le GameObject qui les contient
    private void ReloadInputs()
    {
        inputs = GameObject.Find("Inputs").GetComponent<Inputs>().inputs;
    }
    
    public void TogglePauseMenu()
    {
        if (pauseMenu.activeSelf || optionsMenu.activeSelf)
        //Desactivation du menu
        {
            canMove = true;

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
            canMove = false;

            //Debloque la souris
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            pauseMenu.SetActive(true);
        }
    }

    //Arrete la prise en compte des inputs pendant duration secondes
    public void StopInputs(float duration)
    {
        stopInputsTime = duration;
    }
}