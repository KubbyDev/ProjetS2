
using UnityEngine;

public enum Hero
{
    Stricker = 0,
    Warden = 1,
    Ninja = 2
}

static class Heroes
{
    public static readonly Mesh strickerMesh = Resources.Load<Mesh>("Meshes/Striker");
    public static readonly Mesh wardenMesh = Resources.Load<Mesh>("Meshes/Warden");
    public static readonly Mesh ninjaMesh = Resources.Load<Mesh>("Meshes/Ninja");

    static readonly System.Random rng = new System.Random();
    
    //Renvoie un hero random
    public static Hero Random()
    {
        return (Hero) rng.Next(3);
    }

    //Renvoie le mesh correspondant a un hero
    public static Mesh GetMesh(this Hero h)
    {
        switch (h)
        {
            case Hero.Stricker: return strickerMesh;
            case Hero.Warden: return wardenMesh;
            case Hero.Ninja: return ninjaMesh;
            default: return null;
        }
    }
}