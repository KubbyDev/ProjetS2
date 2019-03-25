using UnityEngine;


public class HookBall : MonoBehaviour
{
    [SerializeField] private int movmentcoef = 1;
    private GameObject hooker;
    private Vector3 direction;
    
    public void UpdateDirection(GameObject p_hooker,Vector3 p_direction)
    {
        this.direction = p_direction;
        this.hooker = p_hooker;
    }

    private void Update()
    {
        transform.position += direction*Time.deltaTime*movmentcoef;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            Ball.UpdatePossessor(hooker);
            Destroy(gameObject);
        }
    }
    
    
}