using Photon.Pun;
using UnityEngine;

public class Freeze : MonoBehaviour
{
    public const float Freeze1_Duration = 1f;  //Duree du freeze complet (pas de recuperation, pas de mouvement)
    public const float Freeze2_Duration = 4f;  //Duree du freeze partiel (pas de mouvement mais recuperation possible)
    public const float lifeTime = 1f;          //Temps de vie du projectile en secondes
    public const float bulletSpeed = 50f;      //Vitesse du projectile
    
    void Start()
    {
        //Detruit le projectile apres lifeTime secondes
        Destroy(this.gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += transform.forward * (Time.deltaTime * bulletSpeed); //envoie la ball dans la direction de la camera
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ball") && Ball.possessor == null)
            Ball.photonView.RPC(
                "Freeze", 
                RpcTarget.All, 
                Ball.ball.transform.position, 
                Freeze1_Duration, 
                Freeze2_Duration, 
                PhotonNetwork.Time);
    }
}
