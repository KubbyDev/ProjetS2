using UnityEngine;

//Cette classe recoit des inputs de PlayerInputManager pour les actions qui font bouger le joueur

public class MovementManager : MonoBehaviour
{
	public float jumpStrength = 1000;
	public float movementSpeed = 5;
    public int maxJumps = 3;
    public float pitchLimit = 60;         //Une valeur entre 0 et 90 svp faites pas chier
    public Transform camAnchor;

    private CharacterController cc;
    private Vector3 velocity;
    private int usableJumps;               //Le nombre de sauts restants (Reset quand le sol est touche)
    private Vector3 movementInput;         //Le mouvement correspondant aux inputs ZQSD

	void Start ()
	{
        cc = GetComponent<CharacterController>();
    }

    private void LateUpdate()
    {
        //Gravity
        //Pour supprimer l'impression de faible gravite on l'augmente quand le joueur tombe
        velocity += Physics.gravity * Time.deltaTime * (velocity.y < 0 ? 1.8f : 1.0f);

        if (cc.isGrounded)
        {
            usableJumps = maxJumps;
            velocity = Vector3.zero;
        }

        //Fait bouger le joueur
        cc.Move(velocity * Time.deltaTime + movementInput);

        movementInput = Vector3.zero;
    }

    public void Move(Vector3 input) 
	{
        input = input.z*transform.forward + input.x*transform.right;
        input.y = 0;
        movementInput = input.normalized * Time.deltaTime * movementSpeed;
    }

    public void Rotate(Vector3 rotation)  //Cette fonction est a refaire proprement
    {
        camAnchor.transform.eulerAngles += rotation;

        transform.rotation = Quaternion.Euler(new Vector3(0, camAnchor.transform.eulerAngles.y, 0));

        //Bloque l'axe vertical
        //TODO: faire ça proprement
        float x = camAnchor.transform.rotation.eulerAngles.x;
        if (x > pitchLimit && x < 180)
            camAnchor.transform.rotation = Quaternion.Euler(pitchLimit, 0, camAnchor.transform.eulerAngles.z);
        if (x < 360-pitchLimit && x > 180)
            camAnchor.transform.rotation = Quaternion.Euler(360-pitchLimit, 0, camAnchor.transform.eulerAngles.z);
    }

    public void Jump()
    {
        if (usableJumps > 0)
        {
            AddForce(new Vector3(0, jumpStrength, 0));
            usableJumps--;
        }
    }

	public void AddForce(Vector3 force)  
	{
        velocity += force;
	}
}
