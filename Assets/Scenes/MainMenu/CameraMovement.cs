using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField][Range(0,90)] private float rotationSpeed = 2;

    private Transform camAnchor;

    void Start()
    {
        camAnchor = this.transform;
    }

    void Update()
    {
        camAnchor.transform.rotation *= Quaternion.Euler(0,rotationSpeed*Time.deltaTime,0);
    }
}
