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

    public static class HeroesMeshes
    {
        public static readonly Mesh strikerMesh = tools.strikerMesh;
        public static readonly Mesh wardenMesh = tools.wardenMesh;
        public static readonly Mesh ninjaMesh = tools.ninjaMesh;

        public static readonly Material[] strikerMaterials = tools.strikerMaterials;
        public static readonly Material[] wardenMaterials = tools.wardenMaterials;
        public static readonly Material[] ninjaMaterials = tools.ninjaMaterials;
    }
    
    // Images des spells -----------------------------------------------------------------------------------------------

    [SerializeField] private Sprite turbo;
    [SerializeField] private Sprite escape;
    [SerializeField] private Sprite magnet;
    [SerializeField] private Sprite freeze;
    [SerializeField] private Sprite explode;
    [SerializeField] private Sprite smoke;

    public static class SpellsSprites
    {
        public static readonly Sprite[] stricker = {tools.turbo, tools.escape};
        public static readonly Sprite[] warden = {tools.magnet, tools.freeze};
        public static readonly Sprite[] ninja = {tools.explode, tools.smoke};
    }
    
    // Materials des Teams ---------------------------------------------------------------------------------------------

    [SerializeField] private Material blueTeam;
    [SerializeField] private Material orangeTeam;
    [SerializeField] private Material noTeam;

    public static class TeamsMaterials
    {
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
