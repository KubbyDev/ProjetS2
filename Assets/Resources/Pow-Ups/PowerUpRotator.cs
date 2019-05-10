using System;
using UnityEngine;

public class PowerUpRotator : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 50;
    [SerializeField] private float oscillationSpeed = 1.5f;
    [SerializeField] private float oscillationAmplitude = 0.5f;
    
    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed*Time.deltaTime);
        transform.localPosition = new Vector3(0, (float) Math.Sin(Time.time * oscillationSpeed) * oscillationAmplitude,0);
    }
}
