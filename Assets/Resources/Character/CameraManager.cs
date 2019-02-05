using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

    [SerializeField] [Range(0,90)] private float pitchLimit = 60;     //L'angle max de camera en vertical
    [SerializeField] [Range(0,10)] private float camDistance = 4;     //La distance entre la camera et la tete du joueur
    [SerializeField] [Range(0, 1)] private float camRigidity = 0.95f; //(En 3e personne) La rigidite de la camera (si cette valeur est basse, les mouvements seront plus fluides)

    private bool isFps;               //true: Premiere personne, false: 3e personne
    private Transform camAnchor;

    void Start()
    {
        camAnchor = transform.Find("CameraAnchor");
    }

    void FixedUpdate()
    {
        if (isFps)
            FirstPerson();
        else
            ThirdPerson();
    }

    public void changeCamera()
    {
        //On passe a l'autre camera
        isFps = !isFps;

        //On affiche l'avatar du joueur en tps, pas en fps
        if (isFps)
            GetComponent<MeshRenderer>().enabled = false;
        else
            GetComponent<MeshRenderer>().enabled = true;
    }

    private void ThirdPerson()
    {
        //On tourne le pivot de la camera dans la bonne orientation
        Camera.main.transform.rotation = camAnchor.transform.rotation;

        Vector3 newPosition;
        //On trace un raycast en arriere
        RaycastHit hitInfo;
        if (Physics.SphereCast(camAnchor.transform.position, 0.25f, -1 * camAnchor.transform.forward, out hitInfo, camDistance +1))
            //s'il touche un mur on place la camera un peu avant le point d'impact
            newPosition = camAnchor.transform.position - Mathf.Min(hitInfo.distance - 0.5f, camDistance) * camAnchor.transform.forward;
        else
            //Si aucun mur n'est detecte, on place la camera a la bonne distance
            newPosition = camAnchor.transform.position - camDistance * camAnchor.transform.forward;

        //On deplace la camera sur sa nouvelle position en appliquant un petit smooth
        Camera.main.transform.position = newPosition * camRigidity + Camera.main.transform.position * (1 - camRigidity);
    }

    private void FirstPerson()
    {
        //On tourne la camera dans la bonne orientation et on la place au bon endroit
        Camera.main.transform.eulerAngles = new Vector3(camAnchor.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
        Camera.main.transform.position = camAnchor.transform.position + new Vector3(0,-0.6f,0);
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

    //Renvoie l'orientation de la camera
    public Quaternion GetRotation()
    {
        return camAnchor.rotation;
    }
}
