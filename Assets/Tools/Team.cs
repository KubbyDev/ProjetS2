
public enum Team
{
    None = -1,
    Blue = 0,
    Orange = 1,
}

static class Teams
{
    static readonly System.Random rng = new System.Random();
    
    public static Team otherTeam(this Team team)
    {
        return (Team) (((int) team+1)%2);
    }

    public static Team Random()
    {
        return (Team) rng.Next(2);
    }
}