using UnityEngine;

//Cette classe recoit des inputs de PlayerInputManager pour les actions qui font bouger le joueur

public class MovementManager : MonoBehaviour
{
    [SerializeField] private float jumpStrength = 10;       //La force des sauts
    [SerializeField] private float movementSpeed = 500;     //La vitesse des deplacements au sol
    [SerializeField] private int maxJumps = 3;              //Le nombre max de sauts sans toucher le sol
    [SerializeField] private float inAirControl = 1.2f;     //La force des inputs en l'air (en l'air: inputs *= inAirControl/vitesse^2)

    private CharacterController cc;
    private Vector3 velocity;             //La vitesse actuelle du joueur
    private int usableJumps;              //Le nombre de sauts restants (Reset quand le sol est touche)

	void Start()
	{
        cc = GetComponent<CharacterController>();
    }

    void FixedUpdate()
    {
        //Gravity
        //Pour supprimer l'impression de faible gravite on l'augmente quand le joueur tombe
        velocity += Physics.gravity * Time.fixedDeltaTime * (velocity.y < 0 ? 2f : 1.0f);

        //Fait bouger le joueur
        cc.Move(velocity * Time.fixedDeltaTime);

        if (cc.isGrounded) //Quand le joueur est au sol
        {
            velocity = Vector3.zero;
            usableJumps = maxJumps;
        }
        else               //Quand le joueur est en l'air
        {
            velocity = cc.velocity;
        }
    }

    //Appellee par InputManager
    public void Move(Vector3 input) 
	{
        input = input.z*transform.forward + input.x*transform.right;
        input.y = 0;

        velocity += input.normalized * Time.fixedDeltaTime * movementSpeed      //Le vecteur d'inputs en temps normal
            * (cc.isGrounded ? 1 : inAirControl / (velocity.sqrMagnitude+2));   //Quand le joueur est en l'air on multiplie par
                                 //inAirControl/velocity.sqrMagnitude, +2 pour eviter la division par 0 et les a coups
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
