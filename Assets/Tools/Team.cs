
public enum Team
{
    Blue = 1,
    Orange = -1
}

static class TeamMethods
{
    public static Team otherTeam(this Team team)
    {
        return (Team) (-1 * (int) team);
    }
}