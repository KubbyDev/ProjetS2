using Photon.Pun;
using UnityEngine;

public class Tools : MonoBehaviour
{
    private static Tools tools;
    private bool initialised = false;
    
    void Awake()
    {
        if (initialised)
        {
            Destroy(this.gameObject);
            return;
        }
        initialised = true;
        
        //Initialisation

        tools = this;
        
        //C'est crade mais j'ai trouve aucune autre solution (Ce script est dans Tools)
        DontDestroyOnLoad(this.gameObject);
    }
    
    // Meshes des Heroes -----------------------------------------------------------------------------------------------
    
    [SerializeField] private Mesh strikerMesh;
    [SerializeField] private Mesh wardenMesh;
    [SerializeField] private Mesh ninjaMesh;

    [SerializeField] private Material[] strikerMaterials;
    [SerializeField] private Material[] wardenMaterials;
    [SerializeField] private Material[] ninjaMaterials;

    [SerializeField] private Material blueTeam;
    [SerializeField] private Material orangeTeam;
    [SerializeField] private Material noTeam;

    public static class HeroesMeshes
    {
        public static readonly Mesh strikerMesh = tools.strikerMesh;
        public static readonly Mesh wardenMesh = tools.wardenMesh;
        public static readonly Mesh ninjaMesh = tools.ninjaMesh;

        public static readonly Material[] strikerMaterials = tools.strikerMaterials;
        public static readonly Material[] wardenMaterials = tools.wardenMaterials;
        public static readonly Material[] ninjaMaterials = tools.ninjaMaterials;

        public static readonly Material blueTeam = tools.blueTeam;
        public static readonly Material orangeTeam = tools.orangeTeam;
        public static readonly Material noTeam = tools.noTeam;
    }
    
    // Calcul de latence -----------------------------------------------------------------------------------------------

    public static float GetLatency(double sendMoment)
    {
        double latency = PhotonNetwork.Time - sendMoment;

        //PhotonNetwork.Time passe a 0 quand il arrive a 4M et des bananes. Cette ligne evite tout probleme
        if (latency < 0)
            latency += 4294967.295;

        return (float) latency;
    }
    
    // Messages sur le menu principal ----------------------------------------------------------------------------------

    public static string message;
}
