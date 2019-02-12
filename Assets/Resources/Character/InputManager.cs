using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private bool invertY = false;
    [SerializeField] [Range(0, 10)] private float sensivityY = 2;
    [SerializeField] [Range(0, 10)] private float sensivityX = 2;

    private MovementManager movement;
    private CameraManager cam;
    private BasicSpell spell;
    private Striker striker;
    private Ninja ninja;
    private BallManager ball;
    private GameObject menus;

    void Start()
    {
        movement = GetComponent<MovementManager>();
        cam = GetComponent<CameraManager>();
        ball = GetComponent<BallManager>();
        spell = GetComponent<BasicSpell>();
        striker = GetComponent<Striker>();
        ninja = GetComponent<Ninja>();
        menus = GameObject.Find("Menus");

        //Bloque la souris
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    void Update()
    {
        //Deplacements (ZQSD)
        Vector3 move = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        movement.Move(move);

        //Rotation de la camera
        Vector3 rot = new Vector3(Input.GetAxisRaw("Mouse X") * sensivityX, Input.GetAxisRaw("Mouse Y") * sensivityY * (invertY ? 1 : -1), 0);
        if (rot.sqrMagnitude > 0)
            cam.Rotate(rot);

        //Sauts
        if (Input.GetKeyDown(KeyCode.Space))
            movement.Jump(move);       //Cette fonction prend en parametre les inputs ZQSD pour les dashes

        //Spells
        if (Input.GetKeyDown(KeyCode.A))
            ninja.Explode_Boost();
        if (Input.GetKeyDown(KeyCode.R))
            striker.Speed();
        if (Input.GetKeyDown(KeyCode.E))
            striker.escape();

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

        //Menu Tab
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            GameObject tabMenu = menus.transform.GetChild(0).gameObject;
            tabMenu.SetActive(!tabMenu.activeSelf);
        }
    }
}