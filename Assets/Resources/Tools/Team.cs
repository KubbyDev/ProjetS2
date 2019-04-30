using System.Collections.Generic;
using UnityEngine;

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

    //Renvoie le Material de la Team (un material bleu pour l'equipe bleu...)
    public static Material GetMaterial(this Team t)
    {
        switch (t)
        {
            case Team.Blue: return Tools.TeamsMaterials.blueTeam;
            case Team.Orange: return Tools.TeamsMaterials.orangeTeam;
            case Team.None: return Tools.TeamsMaterials.noTeam;
            default: return null;
        }
    }

    public static bool IsOpponnentOf(this Team me, Team other)
    {
        if (me == other)
            return me == Team.None;

        return true;
    }
}