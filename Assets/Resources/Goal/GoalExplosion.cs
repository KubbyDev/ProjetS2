using DigitalRuby.LightningBolt;
using UnityEngine;

//Cette classe sert a faire une jolie explosion de but

public class GoalExplosion : MonoBehaviour
{
    [SerializeField] private int lightningCount = 10;
    [SerializeField] private GameObject lightningPrefab;
    [SerializeField] private GameObject goalExplosion;     //Reference a la particule de but
    [SerializeField] private float goalRadius;
    
    //Quand il y a une explosion de but, on fait partir des eclairs de la balle vers des points sur l'anneau
    private Vector3[] positions;     //Ce tableau correspond a toutes les positions de fin des eclairs
    
    void Start()
    {
        Vector3 position = transform.position;
        float increase = 360.0f / lightningCount;
        positions = new Vector3[lightningCount];
        
        for(int i = 0; i < lightningCount; i++)
            positions[i] = new Vector3(
                goalRadius*Mathf.Cos(i*increase) + position.x,
                goalRadius*Mathf.Sin(i*increase) + position.y,
                   position.z);
    }

    //Genere l'explosion de but sur ce but
    public void MakeGoalExplosion(Vector3 ballPosition)
    {
        for (int i = 0; i < lightningCount; i++)
        {
            LightningBoltScript lightning = Instantiate(lightningPrefab, Vector3.zero, Quaternion.identity).GetComponent<LightningBoltScript>();
            lightning.StartPosition = ballPosition;
            lightning.EndPosition = positions[i];
            Destroy(lightning.gameObject, 1.0f);
        }
        
        //Fait pop les particules de goal explosion
        GameObject explosion = Instantiate(goalExplosion, ballPosition, Quaternion.identity);
        
        //Modifie la couleur de l'explosion suivant la team qui a marque
        Color color1 = new Color(1f, 1f, 1f);
        Color color2 = new Color(0.91f, 0.91f, 0.91f);
        if (Ball.script.lastTeam == Team.Blue)
        {
            color1 = new Color(0.36f, 0.87f, 1f);
            color2 = new Color(0.19f, 0.45f, 1f);
        }
        if (Ball.script.lastTeam == Team.Orange)
        {
            color1 = new Color(1f, 0.61f, 0f, 1f);
            color2 = new Color(1f, 0.46f, 0.15f);
        }

        ParticleSystem.MainModule dots = explosion.transform.Find("Dots").GetComponent<ParticleSystem>().main;
        dots.startColor = new ParticleSystem.MinMaxGradient(color1, color2);
        ParticleSystem.MainModule centralCircle = explosion.transform.Find("CentralCircle").GetComponent<ParticleSystem>().main;
        centralCircle.startColor = new ParticleSystem.MinMaxGradient(color1, color2);
        explosion.transform.Find("CentralCircle").GetComponent<ParticleSystemRenderer>().trailMaterial = Ball.script.lastTeam.GetMaterial();
    }
}
