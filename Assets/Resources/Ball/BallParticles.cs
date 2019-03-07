using System.Collections;
using System.Collections.Generic;
using DigitalRuby.LightningBolt;
using UnityEngine;

public class BallParticles : MonoBehaviour
{
    [SerializeField] private GameObject lightningPrefab;                       //La prefab des eclairs
    [SerializeField] [Range(0.01f, 1)] private float lightningLifeTime = 0.2f; //Le temps durant lequel chaque eclair reste
    [SerializeField] [Range(0.01f, 1)] private float spawnPeriod = 0.1f;       //Le temps entre deux spawns d'eclairs

    private float timeToSpawn;  //Le temps restant avant le prochain spawn d'eclair
    
    // Update is called once per frame
    void Update()
    {
        if (timeToSpawn < 0 && Ball.possessor != null)
        {
            //On fait pop l'eclair
            LightningBoltScript lightning = Instantiate(lightningPrefab, Vector3.zero, Quaternion.identity).GetComponent<LightningBoltScript>();
            
            //On lui donne ses positions de debut et de fin
            lightning.StartObject = this.gameObject;           
            lightning.EndObject = Ball.possessor;          
            
            //On le detruit apres lightningLifeTime secondes
            Destroy(lightning.gameObject, lightningLifeTime);
            
            //On remet le temps avant d'en spawn un nouveau a spawnPeriod
            timeToSpawn = spawnPeriod;
        }
        else
        {
            timeToSpawn -= Time.deltaTime;
        }
    }
}
