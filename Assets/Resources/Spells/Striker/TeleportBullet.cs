﻿using Photon.Pun;
using UnityEngine;

public class TeleportBullet : MonoBehaviour
{
    public const float maxTime = 1f;  //Temps de vie de la balle
    
    private GameObject shooter;    //Reference au joueur
    private float startTime;       //Moment du lancer
    [SerializeField] private GameObject theVoidParticle;
    
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
        GetComponent<PhotonView>().RPC("Teleport_RPC", RpcTarget.All, shooter.transform.position, (int) shooter.GetComponent<PlayerInfo>().team);
        
        //Tp le joueur un peu avant le point de collision pour eviter de passer a travers les murs
        shooter.transform.position = transform.position - direction.normalized*1.0f;
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
    public void Teleport_RPC(Vector3 fromPosition, int team)
    {
        GameObject particle = Instantiate(theVoidParticle, fromPosition, Quaternion.identity);
        ParticleSystem.MainModule main = particle.GetComponent<ParticleSystem>().main;
        main.startColor = new ParticleSystem.MinMaxGradient(((Team) team).GetMaterial().color);
        
        Destroy(this.gameObject);
    }
}

    
