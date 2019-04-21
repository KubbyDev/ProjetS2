using UnityEngine;

public enum Hero
{
    Stricker = 0,
    Warden = 1,
    Ninja = 2
}

static class Heroes
{
    static readonly System.Random rng = new System.Random();
    
    //Renvoie un hero random
    public static Hero Random()
    {
        return (Hero) rng.Next(3);
    }

    //Renvoie le mesh correspondant a un hero
    public static Model GetModel(this Hero h)
    {
        switch (h)
        {
            case Hero.Stricker: 
                return new Model {
                    mesh = Tools.HeroesMeshes.strikerMesh, 
                    materials = Tools.HeroesMeshes.strikerMaterials,
                    corePosition = new Vector3(0, 0.282f, 0.15f)
                };
            case Hero.Warden:                 
                return new Model {
                    mesh = Tools.HeroesMeshes.wardenMesh, 
                    materials = Tools.HeroesMeshes.wardenMaterials,
                    corePosition = new Vector3(0, 0.107f, 0.339f)
                };
            case Hero.Ninja:
                return new Model {
                    mesh = Tools.HeroesMeshes.ninjaMesh, 
                    materials = Tools.HeroesMeshes.ninjaMaterials,
                    corePosition = new Vector3(0, 0.256f, 0.091f)
                };
            default: return null;
        }
    }
    
}

public class Model
{
    public Mesh mesh;
    public Material[] materials;
    public Vector3 corePosition;
}