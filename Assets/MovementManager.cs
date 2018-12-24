using UnityEngine;

//Cette classe recoit des inputs de PlayerInputManager pour les actions qui font bouger le joueur

public class MovementManager : MonoBehaviour
{
	public float jumpStrength = 1000;
	public float movementSpeed = 5;
    public int maxJumps = 3;
	
	private Rigidbody rb;
    private int usableJumps;

	void Start ()
	{
		rb = GetComponent<Rigidbody>();
        usableJumps = maxJumps;
    }
	
	public void Move(Vector3 input) 
	{
		transform.Translate(input * Time.deltaTime * movementSpeed);
	}

    public void Jump()
    {
        if (usableJumps > 0) //Verifier qu'il reste des sauts possibles
        {
            rb.AddForce(new Vector3(0, jumpStrength, 0));
            usableJumps--;
        }
    }

	public void AddForce(Vector3 force) 
	{
		rb.AddForce(force);
	}
}
