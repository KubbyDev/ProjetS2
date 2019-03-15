
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ninja : MonoBehaviour
{
    [SerializeField] private float Explode_Spell_Duration = 1f;   //Duree du speed
    [SerializeField] private float Explode_Cooldown = 20f;        //Cooldown du spell
    [SerializeField] private float Explode_Speed_Boost = 2f;      //Force du speed
    [SerializeField] private float Explosion_Radius = 5.0f;       //Rayon dans lequel les joueurs subissent l'explosion

    private PlayerInfo Info;     //Reference au script qui gere la camera du joueur
    [SerializeField] private float Smoke_Spell_Duration = 5f;     //Duree du spell est smoke               
    [SerializeField] private float Smoke_Cooldown = 8f;           //Cooldown du spell smoke
    [SerializeField] private float Temps_distance = 2;            //Duree de l'existence de SmokeBomb avant d'exploser
    [SerializeField] private bool Can_Smoke = true;                //Bool pour savoir si cooldown est finie ou pas
    [SerializeField] private GameObject SmokeBomb;                //Prefab de la bombe pour le smoke
    [SerializeField] private GameObject SmokeExplosion;

    private bool Explode_Off_Cooldown = true;    //true = cooldown de Explode fini
    private MovementManager move;                //Reference au script qui gere les mouvements du joueur

    void Start()
    {
        move = GetComponent<MovementManager>();
        Info = GetComponent<PlayerInfo>();
    }

    public void Explode_Spell()
    {
        StartCoroutine(ExplodeCoroutine());
    }

    IEnumerator ExplodeCoroutine()
    {
        if (Explode_Off_Cooldown)
        {
            move.MultiplySpeed(Explode_Speed_Boost);                             // Augmente la vitesse
            Explode_Off_Cooldown = false;
            yield return new WaitForSeconds(Explode_Spell_Duration);             // Lance le cooldown
            move.MultiplySpeed(1 / Explode_Speed_Boost);                         // Remet la vitesse normale
            Explosion();
            yield return new WaitForSeconds(Explode_Cooldown);
            Explode_Off_Cooldown = true;                                         // Le spell redevient utilisable une fois le cooldown ecoule
        }
    }

    public void Explosion()
    {
        //Renvoie la liste des joueurs presents sur le terrain
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        //Pour chaque joueur
        foreach(GameObject player in players)
        {
            //Si le joueur est trop proche, et que ce n'est pas celui qui a lance le spell
            if (player != this.gameObject && Vector3.Distance(player.transform.position, this.transform.position) <= Explosion_Radius)
            {
                //On le propulse
                Vector3 Blast = new Vector3(player.transform.position.x - transform.position.x, 50f, player.transform.position.z - transform.position.z);
                player.GetComponent<MovementManager>().AddForce(Blast);
            }
        }
    }

    public void Smoke()
    {
        StartCoroutine(SmokeCouroutine());
    }

    IEnumerator SmokeCouroutine()
    {
        if (Can_Smoke)
        {
            GameObject bomb = Instantiate(SmokeBomb, transform.position + new Vector3(0, 1.5f, 0) + transform.forward, Info.cameraAnchor.rotation); //Cree SmokeBomb
            bomb.GetComponent<Rigidbody>().AddForce(bomb.transform.forward * 1000); //Applique une force
            Can_Smoke = false;
            yield return new WaitForSeconds(Temps_distance); //la duree avant l'explosion de la bombe
            GameObject explosion = Instantiate(SmokeExplosion, bomb.transform.position, Quaternion.identity); //cree SmokeExplosion dans la position de SmokeBomb
            Destroy(bomb);
            yield return new WaitForSeconds(Smoke_Spell_Duration); //duree de la bombe
            yield return new WaitForSeconds(Smoke_Cooldown);//duree du cooldown
            Can_Smoke = true;

        }   
    }
    
    
}
