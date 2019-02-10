using Photon.Pun;
using UnityEngine;

public class PlayerSync : MonoBehaviour //,IPunObservable
{


    //Pas terrible, produit un mouvement sacade


    /*
    private PhotonView pv;

    private Vector3 targetPosition;
    private Vector3 oldPosition;
    private Quaternion targetRotation;
    private Quaternion oldRotation;

    private float lastUpdateTime;
    private float timeBetweenUpdates;

    void Start()
    {
        //On initialise tout ca pour eviter les erreurs
        oldPosition = transform.position;
        oldRotation = transform.rotation;
        targetPosition = transform.position;
        targetRotation = transform.rotation;
        lastUpdateTime = 0;

        pv = GetComponent<PhotonView>();
    }

    void Update()
    {
        if (pv.IsMine)
            return;

        //Deplace la position et la rotation de facon fluide
        transform.position = Vector3.Lerp(oldPosition, targetPosition, (Time.time - lastUpdateTime)/timeBetweenUpdates);
        transform.rotation = Quaternion.Lerp(oldRotation, targetRotation, (Time.time - lastUpdateTime)/timeBetweenUpdates);
    }


    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) //Donnees envoyees
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else                  //Donnees recues
        {
            timeBetweenUpdates = Time.time - lastUpdateTime;
            lastUpdateTime = Time.time;

            oldPosition = transform.position;
            oldRotation = transform.rotation;
            targetPosition = (Vector3)stream.ReceiveNext();
            targetRotation = (Quaternion)stream.ReceiveNext();
        }
    }
    */
}
