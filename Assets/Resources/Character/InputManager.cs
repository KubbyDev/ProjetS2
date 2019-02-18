using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private bool invertY = false;                   //Inverse la visee en Y
    [SerializeField] [Range(0, 10)] private float sensivityY = 2;    //Sensi horizontale
    [SerializeField] [Range(0, 10)] private float sensivityX = 2;    //Sensi verticale

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

        //Bloque la souris
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    void Update()
    {
        if(canMove)
        {
            //Deplacements (ZQSD)
            Vector3 move = Input.GetAxisRaw("Vertical")*transform.forward + Input.GetAxisRaw("Horizontal")*transform.right;
            move.y = 0;
            movement.Move(move.normalized);

            //Rotation de la camera
            Vector3 rot = new Vector3(Input.GetAxisRaw("Mouse X") * sensivityX, Input.GetAxisRaw("Mouse Y") * sensivityY * (invertY ? 1 : -1), 0);
            if (rot.sqrMagnitude > 0)
                cam.Rotate(rot);

            //Sauts
            if (Input.GetKeyDown(KeyCode.Space))
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
            if (Input.GetMouseButtonDown(0))
                ball.Catch();
            //Tir
            if (Input.GetMouseButtonDown(1))
                ball.Shoot();
        }

        //Menu Tab
        if (Input.GetKeyDown(KeyCode.Tab))
            tabMenu.SetActive(!tabMenu.activeSelf);
        //Menu Pause (sur Backspace au lieu de escape parce que ça fait de la merde dans l'editeur)
        if (Input.GetKeyDown(KeyCode.Backspace))
            TogglePauseMenu();
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
}