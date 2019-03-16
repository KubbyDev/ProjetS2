
using System.Collections.Generic;

public enum Team
{
    None = -1,
    Blue = 0,
    Orange = 1
}

static class Teams
{
    static readonly System.Random rng = new System.Random();
    
    //Team.Orange.OtherTeam() => Team.Blue
    //Team.Blue.OtherTeam() => Team.Orange
    //Team.None.OtherTeam() => Team.Blue
    public static Team OtherTeam(this Team team)
    {
        return (Team) (((int) team+1)%2);
    }

    //Renvoie une Team aleatoire entre Team.Blue et Team.Orange
    public static Team Random()
    {
        return (Team) rng.Next(2);
    }

    //Permet de faire foreach(Team team in Teams.Each())
    public static List<Team> Each()
    {
        return new List<Team> {Team.Blue, Team.Orange};
    }
}