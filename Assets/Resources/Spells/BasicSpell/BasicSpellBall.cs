﻿using Photon.Pun;
using System.Collections;
using UnityEngine;

public class BasicSpellBall : MonoBehaviour
{
    [SerializeField] private int BulletSpeed = 1;
    [SerializeField] private float SlowMultiplier = 0.8f;
    [SerializeField] private float SlowDuration = 3.5f;
    private Vector3 direction;

    public void UpdateDirection(Vector3 p_direction)
    {
        this.direction = p_direction;
    }

    public void Update()
    {
        transform.position += direction * Time.deltaTime * BulletSpeed;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            StartCoroutine(BasicSlowCoroutine(other.gameObject));
        
        Destroy(this.gameObject);
    }

    IEnumerator BasicSlowCoroutine(GameObject target)
    {
        target.GetComponent<MovementManager>().MultiplySpeed(SlowMultiplier*0.1f);
        yield return new WaitForSeconds(SlowDuration*10);
        target.GetComponent<MovementManager>().MultiplySpeed(1 / SlowMultiplier);
    }
}