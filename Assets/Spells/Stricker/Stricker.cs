using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stricker : MonoBehaviour
{
    private MovementManager movement;
    private float TimeSpeedSpell = 3f;

    void Start()
    {
        movement = GetComponent<MovementManager>();
    }

    void Update()
    {
        
    }

    public void Speed()
    {
        StartCoroutine(SpeedCouroutine());
    }

    IEnumerator SpeedCouroutine()
    {
        movement.MultiplySpeed(200f);
        yield return new WaitForSeconds(TimeSpeedSpell);
        movement.MultiplySpeed(-200f);
    }
}
