using System.Collections;
using System.Collections.Generic;
using DigitalRuby.LightningBolt;
using UnityEngine;

public class GoalExplosion : MonoBehaviour
{
    [SerializeField] private int lightningCount = 10;
    [SerializeField] private GameObject lightningPrefab;
    [SerializeField] private ParticleSystem goalExplosion;     //Reference a la particule de but
    [SerializeField] private float goalRadius;
    
    private Vector3[] positions;
    
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
        Instantiate(goalExplosion, ballPosition, Quaternion.identity);
    }
}
