using UnityEngine;


public class HookBall : MonoBehaviour
{
    public const float Speed = 100.0f;
    
    private GameObject hooker;
    private Vector3 direction;
    private bool searchesForBall; //Ce booleen est a true uniquement sur le client qui a lance le hook
    
    public void UpdateDirection(GameObject p_hooker,Vector3 p_direction, bool p_searchesForBall)
    {
        this.searchesForBall = p_searchesForBall;
        this.direction = p_direction;
        this.hooker = p_hooker;
    }

    private void Update()
    {
        transform.position += Time.deltaTime * Speed * direction;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (searchesForBall && other.CompareTag("Ball"))
        {
            Ball.UpdatePossessor(hooker);
            Destroy(gameObject);
            
            ParticleSystem.MainModule main = hooker.transform.Find("ElectricParticles").GetComponent<ParticleSystem>().main;
            main.duration = 1f;
            main.startColor = hooker.GetComponent<PlayerInfo>().team.GetMaterial().color;
            hooker.transform.Find("ElectricParticles").GetComponent<ParticleSystem>().Play();
        }
    }
}