using Photon.Pun;
using UnityEngine;

public class Freeze : MonoBehaviour
{
    [SerializeField] private float Freeze1_Duration = 1f;  //Duree du freeze complet (pas de recuperation, pas de mouvement)
    [SerializeField] private float Freeze2_Duration = 4f;  //Duree du freeze partiel (pas de mouvement mais recuperation possible)
    [SerializeField] private float speed = 5f;             //Vitesse du projectile
    [SerializeField] private float lifeTime = 1f;          //Temps de vie du projectile en secondes
    
    void Start()
    {
        //Detruit le projectile apres lifeTime secondes
        Destroy(this.gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += transform.forward * Time.deltaTime * speed; //envoie la ball dans la direction de la camera
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
                (float) PhotonNetwork.Time);
    }
}
