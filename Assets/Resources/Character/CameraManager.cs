using UnityEngine;

//Ce script gere la camera

public class CameraManager : MonoBehaviour {

    [SerializeField] [Range(0,90)] private float pitchLimit = 60;     //L'angle max de camera en vertical
    [SerializeField] [Range(0,10)] private float camDistance = 4;     //La distance entre la camera et la tete du joueur

    private bool isFps;                 //true: Premiere personne, false: 3e personne
    private Transform camAnchor;        //Le pivot de la camera
    private MeshRenderer meshRenderer;  //Desactiver ca pour rendre le joueur invisible
    private ParticleSystem particles;   //Le component qui gere les particules rondes sous le joueur
    private PlayerInfo infos;           //Le script qui contient les infos sur le joueur
    private Transform cam;              //La position de la camera

    void Start()
    {
        cam = Camera.main.transform;
        camAnchor = transform.Find("CameraAnchor");
        meshRenderer = GetComponent<MeshRenderer>();
        particles = GetComponent<ParticleSystem>();
        infos = GetComponent<PlayerInfo>();
    }

    void Update()
    {
        //Lance la fonction de positionnement de la camera adaptee en fonction de si le joueur est en FPS ou en TPS
        if (isFps)
            FirstPerson();
        else
            ThirdPerson();

        infos.cameraPosition = cam.position;
        infos.cameraRotation = cam.rotation;
    }

    public void ChangeCamera()
    {
        //On passe a l'autre camera
        isFps = !isFps;

        //On affiche l'avatar et les particules du joueur en tps, pas en fps
        if (isFps)
        {
            meshRenderer.enabled = false;
            particles.Stop();
        }
        else
        {
            meshRenderer.enabled = true;
            particles.Play();
        }  
    }

    private void ThirdPerson()
    {
        //On tourne le pivot de la camera dans la bonne orientation
        cam.rotation = camAnchor.transform.rotation;
        cam.rotation = Quaternion.Euler(new Vector3(cam.rotation.eulerAngles.x - 10, cam.rotation.eulerAngles.y, 0));

        Vector3 newPosition;
        //On trace un raycast en arriere
        if (Physics.SphereCast(camAnchor.transform.position, 0.25f, -1 * camAnchor.transform.forward, out RaycastHit hitInfo, camDistance + 1, LayerMask.NameToLayer("IgnoreCamRaycast")))
            //s'il touche un mur on place la camera un peu avant le point d'impact
            newPosition = camAnchor.transform.position - Mathf.Min(hitInfo.distance - 0.5f, camDistance) * camAnchor.transform.forward;
        else
            //Si aucun mur n'est detecte, on place la camera a la bonne distance
            newPosition = camAnchor.transform.position - camDistance * camAnchor.transform.forward;

        cam.position = newPosition;

        //On affiche l'avatar du joueur seulement si la camera n'est pas dedans
        meshRenderer.enabled = (newPosition - camAnchor.position).sqrMagnitude > 0.5f*0.5f;
    }

    private void FirstPerson()
    {
        //On tourne la camera dans la bonne orientation et on la place au bon endroit
        cam.eulerAngles = new Vector3(camAnchor.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
        cam.position = camAnchor.transform.position - new Vector3(0,0.2f,0);
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
