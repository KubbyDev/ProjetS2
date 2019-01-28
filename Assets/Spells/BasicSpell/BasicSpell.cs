﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicSpell : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Shoot()
    {
        GameObject _object = Instantiate(bulletPrefab, transform.position + new Vector3(0, 1.5f, 0) + transform.forward , transform.rotation);
        _object.GetComponent<Rigidbody>().AddForce(transform.forward * 1000);
    }

}
