
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

    private bool Explode_Off_Cooldown = true;    //true = cooldown de Explode fini
    private MovementManager move;                //Reference au script qui gere les mouvements du joueur

    void Start()
    {
        move = GetComponent<MovementManager>();
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
}
