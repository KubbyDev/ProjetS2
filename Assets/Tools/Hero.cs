
public enum Hero
{
    Stricker = 0,
    Warden = 1,
    Ninja = 2
}

static class Heroes
{
    //TODO: Trouver un moyen propre d'initialiser ca
    /*
    public static Mesh strickerMesh;//Le mesh du stricker
    public static Mesh wardenMesh;  //Le mesh du warden
    public static Mesh ninjaMesh;   //Le mesh du ninja
    */
    
    static readonly System.Random rng = new System.Random();
    
    //Renvoie un hero random
    public static Hero Random()
    {
        return (Hero) rng.Next(3);
    }

    /*
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
    */
}