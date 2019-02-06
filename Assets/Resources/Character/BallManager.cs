using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallManager : MonoBehaviour
{
    [SerializeField][Range(1,20)] private float pullStrength = 9;         //La force avec laquelle la balle est attiree
    [SerializeField][Range(0, 10)] private float launchStrength = 2;      //La force avec laquelle la balle est jetee
    [SerializeField][Range(1, 20)] private float maxCatchDistance = 10;   //La distance max a laquelle la balle peut etre attrapee

    private bool hasBall = false;                                          //Si le joueur a la balle
    private GameObject ball;                                               //Une reference a la balle
    private Rigidbody ballRB;                                              //Le component qui gere les physiques de la balle
    private Transform camAnchor;                                           //Une reference a l'ancre de la camera

    // Reference a la balle ---------------------------------------------------------------------------

    void Start()
    {
        UpdateBallRef();
        camAnchor = transform.Find("CameraAnchor");
        ballRB = ball.GetComponent<Rigidbody>();
    }

    //Met a jour la reference a la balle
    //Cette methode peut etre appellee sans argument: elle cherchera la balle elle meme, ou avec la balle en argument
    public void UpdateBallRef(GameObject newRef = null)
    {
        if (ball == null)
            ball = GameObject.FindGameObjectWithTag("Ball");
        else
            ball = newRef;
    }

    // Recuperation de la balle -----------------------------------------------------------------------

    public void Catch()
    {
        //On regarde si la balle est devant la camera a une distance inferieure a maxCatchDistance
        RaycastHit hitInfo;
        if (Physics.Raycast(camAnchor.transform.position, camAnchor.transform.forward, out hitInfo, maxCatchDistance) && hitInfo.collider.tag == "Ball")
            hasBall = true;
    }

    // Attraction de la balle -------------------------------------------------------------------------

    void Update()
    {
        //Quand le joueur a la balle, on l'attire a lui
        if (hasBall)
            AttractBall();
    }

    void AttractBall()
    {
        ballRB.velocity /= 1.2f;                                                               //Amorti la vitesse
        ballRB.AddForce((transform.position + new Vector3(0,0.0f,0) + 3f*transform.forward     //Un peu devant le torse du joueur
                        - ball.transform.position                                              //Pour que le vecteur aille de la balle au joueur
                        ) * Time.deltaTime * pullStrength * 1000);
    }

    // Tir ---------------------------------------------------------------------------------------------

    public void Shoot()
    {
        if (hasBall)
        {
            hasBall = false;
            ballRB.AddForce(camAnchor.transform.forward * launchStrength * 1000);
        }
    }
}
