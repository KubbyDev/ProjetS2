using UnityEngine;

public class BasicSpellBall : MonoBehaviour
{
    [SerializeField] private int BulletSpeed = 1;
    [SerializeField] private float SlowMultiplier = 0.8f;
    [SerializeField] private float SlowDuration = 3.5f;
    
    private Vector3 direction;
    private GameObject shooter;
    private Team shooterTeam;

    public void Init(Vector3 p_direction, bool searchesForCollisions, GameObject p_shooter)
    {
        //Desactive les collisions sur cette balle
        //Seul le client qui l'a lance check les collisions
        if (!searchesForCollisions)
            GetComponent<SphereCollider>().enabled = false;
        
        direction = p_direction;
        shooter = p_shooter;
        shooterTeam = shooter.GetComponent<PlayerInfo>().team;
    }

    public void Update()
    {
        transform.position += direction * Time.deltaTime * BulletSpeed;
    }

    public void OnTriggerEnter(Collider other)
    {
        //Si c'est un joueur
        if (other.CompareTag("Player"))
        {
            //Si c'est un ennemi
            if (other.gameObject != shooter.gameObject &&
                other.GetComponent<PlayerInfo>().team.IsOpponnentOf(shooterTeam))
            {
                other.GetComponent<MovementManager>().MultiplySpeed(SlowMultiplier, SlowDuration);  
                Destroy(this.gameObject);
            }
        }
        else
            Destroy(this.gameObject);
    }
}