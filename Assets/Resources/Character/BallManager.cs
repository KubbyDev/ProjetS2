using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallManager : MonoBehaviour
{
    [SerializeField] [Range(1, 20)] private float pullStrength = 9;       //La force avec laquelle la balle est attiree
    [SerializeField] [Range(0, 10)] private float launchStrength = 2;     //La force avec laquelle la balle est jetee
    [SerializeField] [Range(1, 20)] private float maxCatchDistance = 6;   //La distance max a laquelle la balle peut etre attrapee
    [SerializeField] [Range(0, 5)] private float catchCooldown = 1;       //Le temps entre 2 tentative pour attraper la balle
    [SerializeField] [Range(0, 5)] private float catchWidth = 1;          //L'imprecision autorisee pour attraper la balle

    [HideInInspector] public bool hasBall = false;                        //Si le joueur a la balle


    private GameObject ball;                                              //Une reference a la balle
    private PhotonView pv;                                                //Le script qui gere ce joueur sur le reseau
    private Rigidbody ballRB;                                             //Le component qui gere les physiques de la balle
    private Transform camAnchor;                                          //Une reference a l'ancre de la camera
    private bool canCatch = true;                                         //Vrai si le joueur peut essayer d'attraper la balle

    // Reference a la balle ---------------------------------------------------------------------------

    void Start()
    {
        UpdateBallRef();
        camAnchor = transform.Find("CameraAnchor");
        ballRB = ball.GetComponent<Rigidbody>();
        pv = GetComponent<PhotonView>();
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
        if (canCatch)
            StartCoroutine(CatchCouroutine());
    }

    IEnumerator CatchCouroutine()
    {
        //On regarde si la balle est devant la camera a une distance inferieure a maxCatchDistance
        foreach (RaycastHit hit in Physics.SphereCastAll(camAnchor.transform.position, catchWidth, camAnchor.transform.forward, maxCatchDistance))
            //On recupere la balle si on la touche ou si on touche son porteur
            if (hit.collider.tag == "Ball" && hit.collider.gameObject.GetComponent<BallCanBeCaught>().canbecaught || hit.collider.tag == "Player" && hit.collider.gameObject.GetComponent<BallManager>().hasBall)
                //On enleve la possession de balle sur tous les joueurs et
                //on la donne au joueur qui vient de la recuperer
                GetComponent<PhotonView>().RPC("CatchBall_RPC", RpcTarget.All);  //Appelle CatchBall_RPC sur chaque client

        canCatch = false;
        yield return new WaitForSeconds(catchCooldown); //la duree du cooldown
        canCatch = true;
    }

    [PunRPC]
    //Cette fonction s'execute sur tous les clients
    private void CatchBall_RPC()
    {
        //On enleve la possession de balle sur tous les joueurs
        foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("Player"))
            gameObject.GetComponent<BallManager>().hasBall = false;

        //On donne la possession de balle au joueur qui vient de la recuperer
        hasBall = true;
    }

    // Attraction de la balle -------------------------------------------------------------------------

    void Update()
    {
        //Quand le joueur a la balle, on l'attire a lui (seulement en local)
        if (hasBall && pv.IsMine)
            AttractBall();

        GetComponent<MeshRenderer>().material.color = Color.white;

        if (hasBall)
            GetComponent<MeshRenderer>().material.color = Color.blue;
    }

    void AttractBall()
    {
        ballRB.velocity /= 1.2f;                                                                 //Amorti la vitesse
        ballRB.AddForce((transform.position + new Vector3(0, 0.0f, 0) + 3f * transform.forward   //Un peu devant le torse du joueur
                        - ball.transform.position                                                //Pour que le vecteur aille de la balle au joueur
                        ) * Time.deltaTime * pullStrength * 1000);
    }

    // Tir ---------------------------------------------------------------------------------------------

    public void Shoot()
    {
        if (hasBall)                                                       //Ce vecteur est passe en parametre du RPC (direction du lancer)
            GetComponent<PhotonView>().RPC("ShootBall_RPC", RpcTarget.All, camAnchor.transform.forward * launchStrength * 1000);
    }

    [PunRPC]
    //Cette fonction s'execute sur tous les clients
    private void ShootBall_RPC(Vector3 launchForce)
    {
        hasBall = false;
        ballRB.AddForce(launchForce);
    }
}
