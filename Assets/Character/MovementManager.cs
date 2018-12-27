using UnityEngine;

//Cette classe recoit des inputs de PlayerInputManager pour les actions qui font bouger le joueur

public class MovementManager : MonoBehaviour
{
	public float jumpStrength = 1000;     //La force des sauts
	public float movementSpeed = 600;     //La vitesse des deplacements au sol
    public int maxJumps = 3;              //Le nombre max de sauts sans toucher le sol
    public float pitchLimit = 60;         //L'angle max de camera en vertical (Entre 0 et 90)
    public float inAirControl = 0.2f;     //Le pourcentage de control en l'air (1 = comme au sol)
    public Transform camAnchor;

    private CharacterController cc;
    private Vector3 velocity;              //La vitesse actuelle du joueur
    private int usableJumps;               //Le nombre de sauts restants (Reset quand le sol est touche)

	void Start()
	{
        cc = GetComponent<CharacterController>();
    }

    private void FixedUpdate()
    {
        //Gravity
        //Pour supprimer l'impression de faible gravite on l'augmente quand le joueur tombe
        velocity += Physics.gravity * Time.fixedDeltaTime * (velocity.y < 0 ? 2f : 1.0f);

        //Fait bouger le joueur
        CollisionFlags cf = cc.Move(velocity * Time.fixedDeltaTime);

        if (cc.isGrounded) //Quand le joueur est au sol
        {
            velocity = Vector3.zero;
            usableJumps = maxJumps;
        }
        else               //Quand le joueur est en l'air
        {
            if (cf != CollisionFlags.None)
                velocity = cc.velocity;

            if (cf != CollisionFlags.None) Debug.Log("Collision");
        }
    }

    //Appellee par InputManager
    public void Move(Vector3 input) 
	{
        input = input.z*transform.forward + input.x*transform.right;
        input.y = 0;
        velocity += input.normalized * Time.fixedDeltaTime * movementSpeed * (cc.isGrounded ? 1 : inAirControl);
    }

    //Appellee par InputManager
    public void Rotate(Vector3 rotation)
    {
        //Tourne le joueur sur l'axe horizontal
        transform.rotation *= Quaternion.Euler(new Vector3(0, rotation.x, 0));

        //Tourne la camera sur l'axe vertical (Et la bloque a <pitchLimit>)
        float newCamRot = camAnchor.transform.localEulerAngles.x + rotation.y;
        if (newCamRot > pitchLimit && newCamRot < 180)
            newCamRot = pitchLimit;
        if (newCamRot < 360 - pitchLimit && newCamRot > 180)
            newCamRot = -pitchLimit;

        camAnchor.transform.localEulerAngles = new Vector3(newCamRot, 0, 0);
    }

    //Appellee par InputManager
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
