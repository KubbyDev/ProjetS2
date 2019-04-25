using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalRotator : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 20;
    
    void Update()
    {
        transform.Rotate(Vector3.right, rotationSpeed*Time.deltaTime);
    }
}
