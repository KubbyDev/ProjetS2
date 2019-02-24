using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spawns
{
    private class Spawn
    {
        public Transform spawn;
        public bool used;
        private float time;     //Quand le spawn est utilise, il met 5 secondes a redevenir utilisable

        public Spawn(Transform t)
        {
            used = false;
            spawn = t;
        }
        
        //Teleporte le joueur sur ce spawn, et le rend utilise
        public void AssignTo(GameObject player)
        {
            used = true;
            time = 5;
            player.transform.SetPositionAndRotation(spawn.position, spawn.rotation);
        }

        //Met a jour le timer
        //Un spawn utilise passe en unused 5 seconde apres
        public void UpdateTimer()
        {
            if (time > 0)
                time -= Time.deltaTime;
            else
                used = false;
        }
    }
    
    private static Spawn[] orange; //La liste des spawns oranges
    private static Spawn[] blue;   //La liste des spawns bleus

    //Cette fonction cherche les spawns sur la map
    public static void FindSpawns()
    {
        //Ces objets contiennent les spawns de chaque camp
        Transform blueSpawn = GameObject.Find("BlueSpawns").transform;
        Transform orangeSpawn = GameObject.Find("OrangeSpawns").transform;
        
        orange = new Spawn[orangeSpawn.childCount];
        blue = new Spawn[blueSpawn.childCount];

        for (int i = 0; i < orange.Length; i++)
            orange[i] = new Spawn(orangeSpawn.GetChild(i));
        
        for (int i = 0; i < blue.Length; i++)
            blue[i] = new Spawn(blueSpawn.GetChild(i));
    }

    //Cette fonction doit etre appellee a chaque frame
    //Elle met a jour les timer des Spawns (quand un spawn est utilise il faut 5 secondes pour le reutiliser)
    public static void UpdateTimers()
    {
        foreach (Spawn s in blue)
            s.UpdateTimer();
        
        foreach (Spawn s in orange)
            s.UpdateTimer();
    }

    //Renvoie un objet Spawn aleatoire de la team specifiee
    private static Spawn GetRandomSpawn(Team team)
    {
        return (team == Team.Blue ? blue : orange)[Random.Range(0, orange.Length)];
    }

    //Place le joueur a un spawn aleatoire de sa team
    public static void AtRandom(GameObject player)
    {
        GetRandomSpawn(player.GetComponent<PlayerInfo>().team).AssignTo(player);
    }

    //Renvoie un objet Spawn non utilise aleatoire
    //Si tous les spawns sont utilises, renvoie un spawn aleatoire
    private static Spawn GetRandomSpawnUnused(Team team)
    {
        //On recupere la liste des spawns de la bonne team non utilises
        List<Spawn> spawns = new List<Spawn>();
        foreach (Spawn spawn in team == Team.Blue ? blue : orange)
            if(!spawn.used)
                spawns.Add(spawn);

        //Si tous les spawns sont utilises, on en renvoie un random
        if(spawns.Count == 0)
            return GetRandomSpawn(team);
        
        //Sinon on renvoie un des spawns non utilises
        return spawns[Random.Range(0, spawns.Count)]; 
    }

    //Place le joueur a un spawn aleatoire non utilise de sa team
    public static void AtRandomUnused(GameObject player)
    {
        GetRandomSpawnUnused(player.GetComponent<PlayerInfo>().team).AssignTo(player);
    }

    //Teleporte tous les joueurs a un spawn de leur team
    public static void AssignSpawns(GameObject[] players)
    {
        foreach(GameObject player in players)
            Spawns.AtRandomUnused(player);
    }
}
