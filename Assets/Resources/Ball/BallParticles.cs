using DigitalRuby.LightningBolt;
using UnityEngine;

//Ce script gere toute la partie graphique de la balle (eclairs vers le player qui la possede etc)

public class BallParticles : MonoBehaviour
{
    [SerializeField] private GameObject lightningPrefab;                       //La prefab des eclairs
    [SerializeField] private GameObject frozenParticles;
    [SerializeField] [Range(0.01f, 1)] private float lightningLifeTime = 0.2f; //Le temps durant lequel chaque eclair reste
    [SerializeField] [Range(0.01f, 1)] private float spawnPeriod = 0.1f;       //Le temps entre deux spawns d'eclairs

    private float timeToSpawn;  //Le temps restant avant le prochain spawn d'eclair
    private MeshRenderer meshRenderer;
    private float FrozenCooldown;
    private Light light;
    
    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        light = GetComponent<Light>();
    }

    void Update()
    {
        //Si le temps est ecoule et que quelqu'un a la balle
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

        if (FrozenCooldown > 0)
        {
            FrozenCooldown -= Time.deltaTime;
            if (FrozenCooldown < 0)
                this.transform.Find("SnowParticle").GetComponent<ParticleSystem>().Play();
        }
    }

    public void UpdateColor(bool lastTeamIsBlue)
    {
        meshRenderer.material = (lastTeamIsBlue ? Team.Blue : Team.Orange).GetMaterial();
        light.color = (lastTeamIsBlue ? Team.Blue : Team.Orange).GetMaterial().color;
    }

    public void ResetColor()
    {
        meshRenderer.material = Team.None.GetMaterial();
        light.color = Team.None.GetMaterial().color;
    }
    
    public void OnFreeze(float freeze1, float freeze2)
    { 
        ParticleSystem.MainModule Frozen = this.transform.Find("FrozenParticle").GetComponent<ParticleSystem>().main;
        ParticleSystem.MainModule Snow = this.transform.Find("SnowParticle").GetComponent<ParticleSystem>().main;
        
        Frozen.duration = freeze1;
        Snow.duration = freeze2;

        FrozenCooldown = freeze1;
        
        this.transform.Find("FrozenParticle").GetComponent<ParticleSystem>().Play();
    }

    public void OnFreezeStop()
    {
        
    }
}
