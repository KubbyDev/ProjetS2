using Photon.Pun;
using UnityEngine;

public class TeleportBullet : MonoBehaviour
{
    public const float maxTime = 1f;  //Temps de vie de la balle
    
    private GameObject shooter;    //Reference au joueur
    private float startTime;       //Moment du lancer
    
    //True: Cette balle va regarder si elle doit faire une TP (check les collisions et le temps)
    //Si la balle est inactive c'est parce qu'elle appartient a un autre client (seul le client qui lance la balle la calcule)
    private bool active;
    //La direction sert a tp un peu derriere le point de collision, pour eviter de passer a travers les murs
    private Vector3 direction;
    
    void Update()
    {
        //Quand la balle arrive a la fin de son temps de vie
        if (active && Time.time - startTime >= maxTime)
            Tp();
    }

    //Quand la balle entre en collision avec un truc
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject != shooter)
            Tp();
    }

    //Tp le joueur sur la balle
    private void Tp()
    {
        //Tp le joueur un peu avant le point de collision pour eviter de passer a travers les murs
        shooter.transform.position = transform.position - direction.normalized*1.0f;
        GetComponent<PhotonView>().RPC("Destroy_RPC", RpcTarget.All);
    }

    //Appellee par Stricker.cs
    public void Init(GameObject shooter, float startTime, bool active, Vector3 direction)
    {
        this.direction = direction;
        this.active = active;
        
        if (active)
        {
            this.startTime = startTime;
            this.shooter = shooter;   
        }
        else
            GetComponent<SphereCollider>().enabled = false;
    }

    [PunRPC]
    public void Destroy_RPC()
    {
        Destroy(this.gameObject);
    }
}

    
