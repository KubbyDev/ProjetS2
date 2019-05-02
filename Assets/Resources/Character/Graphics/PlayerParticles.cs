using DigitalRuby.LightningBolt;
using UnityEngine;

/*
 * Ce script gere tous les effets visuels sur le joueur
 */
public class PlayerParticles : MonoBehaviour
{
    private PlayerInfo infos;  
    
    private float timeToSpawn;  //Le temps restant avant le prochain spawn d'eclair
    
    void Awake()
    {
        infos = GetComponent<PlayerInfo>();
    }
    
    void Update()
    {
        // CORE LIGHTNINGS
        if (timeToSpawn < 0)
        {
            for(int i = 0; i < countPerSpawn; i++)
                //GenerateCoreLightning();
            timeToSpawn = spawnPeriod;
        }
        else
            timeToSpawn -= Time.deltaTime;
    }
    
    // CORE LIGHTNINGS -------------------------------------------------------------------------------------------------

    [SerializeField] private GameObject lightningPrefab;     //La prefab des eclairs
    [SerializeField] private float lightningLifeTime = 0.2f; //Le temps durant lequel chaque eclair reste
    [SerializeField] private int countPerSpawn = 10;         //Le nombre d'eclairs qui spawn a chaque fois
    [SerializeField] private float spawnPeriod = 0.01f;      //Le temps entre deux spawns d'eclairs
    [SerializeField] private float lightningLength = 0.25f;  //La longueur des eclairs
    [SerializeField] private float lightningWidth = 0.02f;   //L'epaisseur des eclairs
    
    private void GenerateCoreLightning()
    {
        //On fait pop l'eclair
        GameObject lightning = Instantiate(lightningPrefab, Vector3.zero, Quaternion.identity, this.transform);
        LightningBoltScript lightningScript = lightning.GetComponent<LightningBoltScript>();
        LineRenderer lineRenderer = lightning.GetComponent<LineRenderer>();

        //Rend l'eclair plus fin
        lineRenderer.startWidth = lightningWidth;
        lineRenderer.endWidth = lightningWidth;
        
        //La position de debut est la position du joueur + la position du core par rapport au joueur
        //La position de fin est la position du joueur + la position choisie (qui dépend du joueur)
        Vector3 corePosition = infos.hero.GetModel().corePosition;
        lightningScript.StartObject = this.gameObject;
        lightningScript.EndObject = this.gameObject;
        lightningScript.StartPosition = transform.rotation*corePosition;
        lightningScript.EndPosition = transform.rotation*corePosition + lightningLength*(Random.rotation * Vector3.forward);
        
        //On le detruit apres lightningLifeTime secondes
        Destroy(lightning.gameObject, lightningLifeTime);
    }
}