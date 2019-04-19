using UnityEngine;

public class HeroesMesh : MonoBehaviour
{
    public static HeroesMesh heroesMeshes = null;

    [SerializeField] public Mesh strikerMesh;
    [SerializeField] public Mesh wardenMesh;
    [SerializeField] public Mesh ninjaMesh;

    [SerializeField] public Material[] strikerMaterials;
    [SerializeField] public Material[] wardenMaterials;
    [SerializeField] public Material[] ninjaMaterials;

    [SerializeField] public Material blueTeam;
    [SerializeField] public Material orangeTeam;
    [SerializeField] public Material noTeam;
    
    private void Awake()
    {
        if(heroesMeshes != null)
            Destroy(this.gameObject);
        
        heroesMeshes = this;
    
        //C'est crade mais j'ai trouve aucune autre solution (Ce script est dans Tools)
        DontDestroyOnLoad(this.gameObject);
    }
}
