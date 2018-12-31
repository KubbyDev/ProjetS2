﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

    [SerializeField] private float pitchLimit = 60;         //L'angle max de camera en vertical (Entre 0 et 90)
    [SerializeField] private float camDistance = 4;         //La distance entre la camera et la tete du joueur
    [SerializeField] private float camRigidity = 0.95f;     //La rigidite de la camera (si cette valeur est basse, les mouvements seront plus fluides)
    [SerializeField] private Transform camAnchor;

    void FixedUpdate()
    {
        //On tourne le pivot de la camera dans la bonne orientation
        Camera.main.transform.rotation = camAnchor.transform.rotation;

        Vector3 newPosition;
        //On trace un raycast en arriere
        RaycastHit hitInfo;
        if (Physics.Raycast(camAnchor.transform.position, -1 * camAnchor.transform.forward, out hitInfo, camDistance+1))
            //s'il touche un mur on place la camera un peu avant le point d'impact
            newPosition = camAnchor.transform.position - Mathf.Min(hitInfo.distance-0.5f, camDistance) * camAnchor.transform.forward;
        else
            //Si aucun mur n'est detecte, on place la camera a la bonne distance
            newPosition = camAnchor.transform.position - camDistance * camAnchor.transform.forward;

        //On deplace la camera sur sa nouvelle position en appliquant un petit smooth
        Camera.main.transform.position = newPosition * camRigidity + Camera.main.transform.position * (1 - camRigidity);
    }

    //Appellee par InputManager
    public void Rotate(Vector3 rotation)
    {
        //Tourne le joueur sur l'axe horizontal
        transform.rotation *= Quaternion.Euler(new Vector3(0, rotation.x, 0));

        //Tourne la camera sur l'axe vertical (Et la bloque a <pitchLimit>)
        float newCamRot = camAnchor.transform.localEulerAngles.x + rotation.y;
        if (newCamRot > pitchLimit && newCamRot < 180)
            newCamRot = pitchLimit;
        if (newCamRot < 360 - pitchLimit && newCamRot > 180)
            newCamRot = -pitchLimit;

        camAnchor.transform.localEulerAngles = new Vector3(newCamRot, 0, 0);
    }
}