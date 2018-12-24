using UnityEngine;
using UnityEngine.Networking;

public class InputManager : NetworkBehaviour
{
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
    }
}