using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private bool invertY = false;
    [SerializeField] private float sensivityY = 2;
    [SerializeField] private float sensivityX = 2;

    private MovementManager movement;
    private CameraManager cam;

    void Start()
    {
        movement = GetComponent<MovementManager>();
        cam = GetComponent<CameraManager>();
    }
    
    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            movement.Jump();

        movement.Move(new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")));
        cam.Rotate(new Vector3(Input.GetAxisRaw("Mouse X") * sensivityX,  Input.GetAxisRaw("Mouse Y") * sensivityY * (invertY ? 1 : -1), 0));
    }
}