using UnityEngine;
using UnityEngine.Networking;

public class InputManager : NetworkBehaviour
{
    public bool invertY = false;
    public float sensivityY = 2;
    public float sensivityX = 2;

    private MovementManager movement;
    private Spell_BasicBullet basicBullet;

    void Start()
    {
        movement = GetComponent<MovementManager>();
        basicBullet = GetComponent<Spell_BasicBullet>();
    }
    
    void Update()
    {
        if (!isLocalPlayer)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
            movement.Jump();

        if (Input.GetKeyDown(KeyCode.A))
            basicBullet.Fire();

        movement.Move(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")));
        movement.Rotate(new Vector3(Input.GetAxis("Mouse Y") * sensivityY * (invertY ? 1 : -1), Input.GetAxis("Mouse X") * sensivityX, 0));
    }
}