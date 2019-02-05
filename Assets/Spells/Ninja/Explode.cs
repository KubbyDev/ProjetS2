
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ninja : MonoBehaviour
{
    private MovementManager move;
    [SerializeField] private float Explode_Spell_Duration = 1f;
    [SerializeField] private float Explode_Cooldown = 20f;
    [SerializeField] private float Explode_Speed_Boost = 2f;
    private bool Explode_Off_Cooldown = true;

    [SerializeField] private float Explosion_Radius = 5.0F;
    [SerializeField] private float Explosion_Power = 10.0F;


    // Start is called before the first frame update
    void Start()
    {
        move = GetComponent<MovementManager>();

    }

    // Update is called once per frame
    void Update()
    {

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
        Vector3 Explosion_Center = transform.position;                          // Recupere la position du joueur
        Collider[] colliders = Physics.OverlapSphere(Explosion_Center, Explosion_Radius); // Rercupere les objets dans le rayon de l'explosion
        foreach (Collider hit in colliders)
        {
            if (hit.tag == "Player")
            {
                Vector3 Blast = new Vector3(hit.transform.position.x - transform.position.x, hit.transform.position.y - transform.position.y, 3f);
                hit.GetComponent<MovementManager>().AddForce(Blast);
            }
        }

    }

}
