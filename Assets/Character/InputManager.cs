using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private bool invertY = false;
    [SerializeField] private float sensivityY = 2;
    [SerializeField] private float sensivityX = 2;

    private MovementManager movement;
    private Spell_BasicBullet basicBullet;

    void Start()
    {
        movement = GetComponent<MovementManager>();
        basicBullet = GetComponent<Spell_BasicBullet>();
    }
    
    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            movement.Jump();

        if (Input.GetKeyDown(KeyCode.A))
            basicBullet.Fire();

        movement.Move(new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")));
        movement.Rotate(new Vector3(Input.GetAxisRaw("Mouse X") * sensivityX,  Input.GetAxisRaw("Mouse Y") * sensivityY * (invertY ? 1 : -1), 0));
    }
}