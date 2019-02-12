
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ninja : MonoBehaviour
{
    [SerializeField] private float Explode_Spell_Duration = 1f;
    [SerializeField] private float Explode_Cooldown = 20f;
    [SerializeField] private float Explode_Speed_Boost = 2f;
    [SerializeField] private float Explosion_Radius = 5.0F;
    [SerializeField] private float Explosion_Power = 10.0F;

    private bool Explode_Off_Cooldown = true;
    private MovementManager move;

    // Start is called before the first frame update
    void Start()
    {
        move = GetComponent<MovementManager>();
    }

    public void Explode_Boost()
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
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach(GameObject player in players)
        {
            if (player != this.gameObject && Vector3.Distance(player.transform.position, this.transform.position) <= Explosion_Radius)
            {
                Vector3 Blast = new Vector3(player.transform.position.x - transform.position.x, 50f, player.transform.position.z - transform.position.z);
                player.GetComponent<MovementManager>().AddForce(Blast);
            }
        }
    }
}
