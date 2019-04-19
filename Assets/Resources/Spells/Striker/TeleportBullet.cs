using Photon.Pun;
using UnityEngine;

public class TeleportBullet : MonoBehaviour
{
    [SerializeField] private float maxTime = 2f;
    
    private GameObject shooter;    //Reference au joueur
    private float startTime;       //Moment du lancer
    
    //True: Cette balle va regarder si elle doit faire une TP (check les collisions et le temps)
    //Si la balle est inactive c'est parce qu'elle appartient a un autre client (seul le client qui lance la balle la calcule)
    private bool active;           
    
    void Update()
    {
        //Quand la balle arrive a la fin de son temps de vie
        if (active && Time.time - startTime >= maxTime)
            Tp();
    }

    //Quand la balle entre en collision avec un truc
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject != this.gameObject)
            Tp();
    }

    //Tp le joueur sur la balle
    private void Tp()
    {
        shooter.transform.position = transform.position;
        GetComponent<PhotonView>().RPC("Destroy_RPC", RpcTarget.All);
    }

    //Appellee par Stricker.cs
    public void Init(GameObject shooter, float startTime, bool active)
    {
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

    
