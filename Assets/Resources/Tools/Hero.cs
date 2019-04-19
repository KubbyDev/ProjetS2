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
            case Hero.Stricker: return new Model(HeroesMesh.heroesMeshes.strikerMesh, HeroesMesh.heroesMeshes.strikerMaterials);
            case Hero.Warden: return new Model(HeroesMesh.heroesMeshes.wardenMesh, HeroesMesh.heroesMeshes.wardenMaterials);
            case Hero.Ninja: return new Model(HeroesMesh.heroesMeshes.ninjaMesh, HeroesMesh.heroesMeshes.ninjaMaterials);
            default: return null;
        }
    }
    
}

public class Model
{
    public Mesh mesh;
    public Material[] materials;

    public Model(Mesh mesh, Material[] materials)
    {
        this.mesh = mesh;
        this.materials = materials;
    }
}