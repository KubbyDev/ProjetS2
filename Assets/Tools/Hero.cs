
public enum Hero
{
    Stricker = 0,
    Warden = 1,
    Ninja = 2
}

static class Heros
{
    static readonly System.Random rng = new System.Random();
    
    public static Hero Random()
    {
        return (Hero) rng.Next(3);
    }
}
