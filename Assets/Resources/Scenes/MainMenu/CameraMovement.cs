using UnityEngine;

//Cette classe gere la camera dans le menu principal

public class CameraMovement : MonoBehaviour
{
    [SerializeField][Range(0,90)] private float rotationSpeed = 2;  //Vitesse de rotation de la camera dans le menu

    private Transform camAnchor;          //Reference a l'ancre de la camera dans la scene du menu principal

    void Start()
    {
        camAnchor = this.transform;
    }

    void Update()
    {
        //Tourne la camera
        camAnchor.transform.rotation *= Quaternion.Euler(0,rotationSpeed*Time.deltaTime,0);
    }
}
