using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

//Cette classe permet de faire spawn les joueurs sur le terrain

public class Spawns
{    
    public static int randomSeed;  //Utilisee pour la generation de pseudo random
    //Cette classe utilise un generateur LCG qui genere des nombres pseudo random
    //Si deux instances ont le meme seed elles genereront les memes nombres
    
    private static Spawn[] orange; //La liste des spawns oranges
    private static Spawn[] blue;   //La liste des spawns bleus
    
    //Permet de creer un spawn
    //Chaque objet Spawn contient une position et un boolean used
    //Quand used est a true, ce spawn ne peut plus etre utilise
    private class Spawn
    {
        public bool used;
        private readonly Transform spawn;

        public Spawn(Transform t)
        {
            used = false;
            spawn = t;
        }
        
        //Teleporte le joueur sur ce spawn, et le rend utilise
        public void AssignTo(GameObject player)
        {
            used = true;
            player.transform.SetPositionAndRotation(spawn.position, spawn.rotation);
        }
    }
    
    //Cette fonction cherche les spawns sur la map
    public static void FindSpawns()
    {
        //Ces objets contiennent les spawns de chaque camp
        Transform blueSpawn = GameObject.Find("BlueSpawns").transform;
        Transform orangeSpawn = GameObject.Find("OrangeSpawns").transform;
        
        orange = new Spawn[orangeSpawn.childCount];
        blue = new Spawn[blueSpawn.childCount];

        //Recupere tous les spawns oranges
        for (int i = 0; i < orange.Length; i++)
            orange[i] = new Spawn(orangeSpawn.GetChild(i));
        
        //Recupere tous les spawns bleus
        for (int i = 0; i < blue.Length; i++)
            blue[i] = new Spawn(blueSpawn.GetChild(i));
    }

    //Cette fonction permet aux spawns utilises d'etre utilisables a nouveau
    public static void ResetUsage()
    {
        foreach (Spawn s in blue)
            s.used = false;
        
        foreach (Spawn s in orange)
            s.used = false;
    } 

    //Renvoie un objet Spawn aleatoire de la team specifiee
    private static Spawn GetRandomSpawn(Team team)
    {
        return (team == Team.Blue ? blue : orange)[Random(orange.Length)];
    }

    //Renvoie un objet Spawn non utilise aleatoire
    //Si tous les spawns sont utilises, renvoie un spawn aleatoire
    private static Spawn GetRandomSpawnUnused(Team team)
    {
        //On recupere la liste des spawns de la bonne team non utilises
        List<Spawn> spawns = (team == Team.Blue ? blue : orange).Where(spawn => !spawn.used).ToList();

        //Si tous les spawns sont utilises, on en renvoie un random
        if(spawns.Count == 0)
            return GetRandomSpawn(team);
        
        //Sinon on renvoie un des spawns non utilises
        return spawns[Random(spawns.Count)]; 
    }

    //Place le joueur a un spawn aleatoire non utilise de sa team
    public static void AtRandomUnused(GameObject player)
    {
        GetRandomSpawnUnused(player.GetComponent<PlayerInfo>().team).AssignTo(player);
    }

    //Teleporte tous les joueurs a un spawn de leur team
    public static void AssignSpawns(GameObject[] playersList)
    {
        List<GameObject> players = playersList.ToList();

        //Le ViewID est l'Identifiant du PhotonView du joueur, il est donc unique, et c'est le meme chez tous les clients
        //Il peut donc etre utilise pour traiter tous les joueurs dans le meme ordre chez tous les clients
        //Cette for va donc faire spawn tous les joueurs, du plus petit au plus grand viewID
        for (int i = 0; i < playersList.Length; i++)
        {
            GameObject player = GetSmallestViewID(players);
            players.Remove(player);
            Spawns.AtRandomUnused(player);
        }
    }

    //Cette methode renvoie le joueur ayant le plus petit viewID de la liste
    private static GameObject GetSmallestViewID(List<GameObject> players)
    {
        int min = Int32.MaxValue;
        GameObject player = null;

        foreach (GameObject p in players)
        {
            int id = p.GetPhotonView().ViewID;
            if (min > id)
            {
                min = id;
                player = p;
            }
        }

        return player;
    }
    
    //J'ai refait un random comme ca il suffit de synchroniser le seed et tous les clients auront le meme random
    public static int Random(int max)
    {
        randomSeed = (randomSeed * 131217 + 281) % 1000;
        return randomSeed * max / 1000;
    }
}
