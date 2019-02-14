using Photon.Pun;
using System.Collections;
using UnityEngine;

public class BallManager : MonoBehaviour
{
    [SerializeField] [Range(0, 10)] private float launchStrength = 2;     //La force avec laquelle la balle est jetee
    [SerializeField] [Range(1, 20)] private float maxCatchDistance = 6;   //La distance max a laquelle la balle peut etre attrapee
    [SerializeField] [Range(0, 5)] private float catchCooldown = 1;       //Le temps entre 2 tentative pour attraper la balle
    [SerializeField] [Range(0, 5)] private float catchWidth = 1;          //L'imprecision autorisee pour attraper la balle

    [HideInInspector] public bool hasBall = false;                        //Si le joueur a la balle

    private GameObject ballObject;                                        //Une reference a la balle
    private Ball ball;                                                    //Une reference au script Ball de la balle
    private Transform camAnchor;                                          //Une reference a l'ancre de la camera
    private bool canCatch = true;                                         //Vrai si le joueur peut essayer d'attraper la balle

    void Start()
    {
        UpdateBallRef();
        camAnchor = transform.Find("CameraAnchor");
        ball = ballObject.GetComponent<Ball>();
    }

    //Met a jour la reference a la balle
    //Cette methode peut etre appellee sans argument: elle cherchera la balle elle meme, ou avec la balle en argument
    public void UpdateBallRef(GameObject newRef = null)
    {
        if (ballObject == null)
            ballObject = GameObject.FindGameObjectWithTag("Ball");
        else
            ballObject = newRef;
    }

    //Recuperation de la balle
    public void Catch()
    {
        if (canCatch)
            StartCoroutine(CatchCouroutine());
    }

    IEnumerator CatchCouroutine()
    {
        //On regarde si la balle est devant la camera a une distance inferieure a maxCatchDistance
        foreach (RaycastHit hit in Physics.SphereCastAll(camAnchor.transform.position, catchWidth, camAnchor.transform.forward, maxCatchDistance))
            //On recupere la balle si on la touche ou si on touche son porteur
            if (hit.collider.tag == "Ball" && ball.canBeCaught || hit.collider.tag == "Player" && hit.collider.gameObject.GetComponent<BallManager>().hasBall)
                //On enleve la possession de balle sur tous les joueurs et
                //on la donne au joueur qui vient de la recuperer
                ball.UpdatePossessor(this.gameObject);

        canCatch = false;
        yield return new WaitForSeconds(catchCooldown); //la duree du cooldown
        canCatch = true;
    }

    //Debug: Le joueur qui tient la balle devient bleu
    void Update()
    {
        GetComponent<MeshRenderer>().material.color = Color.white;
        if (hasBall)
            GetComponent<MeshRenderer>().material.color = Color.blue;
    }

    //Lance la balle devant lui
    public void Shoot()
    {
        if (hasBall)
            ball.Shoot(camAnchor.transform.forward * launchStrength * 1000);
    }
}
