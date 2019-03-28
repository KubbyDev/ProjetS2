using Photon.Pun;
using System.Collections;
using UnityEngine;

public class BasicSpellBullet : MonoBehaviour
{
    [SerializeField] private int movmentcoef = 1;
    private float SlowDivider = 0.8f;
    private float SlowDuration = 3.5f;
    private GameObject basicattack;
    private Vector3 direction;

    public void UpdateDirection(GameObject p_basicattack, Vector3 p_direction)
    {
        this.direction = p_direction;
        this.basicattack = p_basicattack;
    }

    public void Update()
    {
        transform.position += direction * Time.deltaTime * movmentcoef;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            BasicSlowCoroutine(other.gameObject);
        }
        Destroy(gameObject);
    }

    IEnumerator BasicSlowCoroutine(GameObject target)
    {
        target.GetComponent<MovementManager>().MultiplySpeed(SlowDivider);
        yield return new WaitForSeconds(SlowDuration);
        target.GetComponent<MovementManager>().MultiplySpeed(1 / SlowDivider);
    }



}