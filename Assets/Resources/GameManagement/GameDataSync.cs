using Photon.Pun;
using Photon.Realtime;
using UnityEditor;
using UnityEngine;

//Ce script transmet les ordres du GameManagerHost vers les clients

public class GameDataSync : MonoBehaviour
{
    private static PhotonView pv;       //Reference au script qui gere les echanges sur le reseau

    void Awake()
    {
        pv = GetComponent<PhotonView>();
    }
    
    // Packet de connection --------------------------------------------------------------------------------------------

    //Cette methode est appellee sur le Host quand un joueur rejoint la salle
    //Elle met a jour le GameData du joueur qui vient de rejoindre
    public static void SendFirstPacket(Player player)
    {        
        pv.RPC("GetFirstPacket_RPC", player, FirstPacket());
    }
    
    private static object[] FirstPacket()
    {
        //On envoie le moment auquel le message part, le temps restant, la config de partie, la seed du LCG des spawns
        return new object[]
        { 
            PhotonNetwork.Time,               //Le moment auquel le message part (pour prendre en compte le temps de trajet)
            PreGameManager.timeLeftToStart,   //Le temps restant avant le debut de la partie
            Spawns.randomSeed                 //La seed du LCG des Spawns (Pour que tous les clients aient les memes nombres random)
        };
    }

    [PunRPC]
    //Cette methode est appellee sur le joueur qui vient de rejoindre la salle, pour mettre a jour son GameData
    private void GetFirstPacket_RPC(double sendMoment, float time, int spawnsSeed)
    {      
        Debug.Log("1st packet received " + time + " " + Tools.GetLatency(sendMoment) + " time = " + PhotonNetwork.Time + " send = " + sendMoment);
        
        //On met a jour le temps restant avant le debut de la game 
        //En prenant en compte le temps de trajet du message
        PreGameManager.timeLeftToStart =  time - Tools.GetLatency(sendMoment);
        
        //La seed du LCG des Spawns (Pour que tous les clients aient les memes nombres random)
        Spawns.randomSeed = spawnsSeed;
    }
    
    // Packet de debut de game -----------------------------------------------------------------------------------------
    
    //Cette methode est appellee sur le Host quand la partie demarre
    public static void SendStartGameEvent()
    {
        pv.RPC("GetStartGameEvent_RPC", RpcTarget.Others, StartGameEvent());
    }
    
    private static object[] StartGameEvent()
    {
        //On envoie le moment auquel le message part, le temps restant, la config de partie, la seed du LCG des spawns
        return new object[]
        { 
            PhotonNetwork.Time,   //Le moment auquel le message part (pour prendre en compte le temps de trajet)
            Spawns.randomSeed     //La seed du LCG des Spawns (Pour que tous les clients aient les memes nombres random)
        };
    }

    [PunRPC]
    //Cette methode est appellee sur le joueur qui vient de rejoindre la salle, pour mettre a jour son GameData
    private void GetStartGameEvent_RPC(double sendMoment, int spawnsSeed)
    {      
        //On met a jour le temps restant avant le debut de la game 
        //En prenant en compte le temps de trajet du message
        PreGameManager.timeLeftToStart =  (float) (3 - (PhotonNetwork.Time - sendMoment));
        
        //La seed du LCG des Spawns (Pour que tous les clients aient les memes nombres random)
        Spawns.randomSeed = spawnsSeed;
        
        //Informe le pregame manager que la partie demarre
        PreGameManager.script.StartGame();
    }
    
    // Packet de but ---------------------------------------------------------------------------------------------------

    //Cette methode est appellee sur le host quand il y a un but (pour confirmer le but aux clients)
    public static void SendOnGoalEvent(bool isBlue, Vector3 ballPosition)
    {
        pv.RPC("GetOnGoalEvent_RPC", RpcTarget.Others, OnGoalEvent(isBlue, ballPosition));
    }

    private static object[] OnGoalEvent(bool isBlue, Vector3 ballPosition)
    {
        return new object[]
        {
            isBlue,                          //True: Le but est dans le but bleu (donc marque par les oranges)
            ballPosition,                    //La position de la balle quand elle est entree dans le but
            PhotonNetwork.Time,              //Le moment auquel le message part (pour prendre en compte le temps de trajet)
            GameManager.timeLeft,            //Le temps restant avant la fin de la partie
            Spawns.randomSeed                //La seed du LCG des Spawns (Pour que tous les clients aient les memes nombres random)
        };
    }
    
    [PunRPC]
    //Cette methode est appellee sur les clients au moment des buts pour les confirmer
    public void GetOnGoalEvent_RPC(bool isBlue, Vector3 ballPosition, double sendMoment, float pTimeLeft, int spawnsSeed)
    {
        //On met a jour le temps restant avant l'engagement
        //En prenant en compte le temps de trajet du message
        GameManager.timeLeftForKickoff = (float) (8 - (PhotonNetwork.Time - sendMoment));
        
        GameManager.timeLeft = pTimeLeft;
        Spawns.randomSeed = spawnsSeed;
        
        //On informe le GameManager qu'il y a eu un but
        GameManager.script.OnGoal(isBlue, ballPosition);
    }
    
    // Packets de fin de game -------------------------------------------------------------------------------------------
    
    //Cette methode est appellee sur le host si il y a un overtime
    public static void SendOvertimeEvent()
    {
        pv.RPC("GetOvertimeEvent_RPC", RpcTarget.Others, OvertimeEvent());
    }

    private static object[] OvertimeEvent()
    {
        return new object[]
        {
            PhotonNetwork.Time,              //Le moment auquel le message part (pour prendre en compte le temps de trajet)
            Spawns.randomSeed                //La seed du LCG des Spawns (Pour que tous les clients aient les memes nombres random)
        };
    }

    [PunRPC]
    //Cette methode est appellee sur les clients si il y a un overtime
    public void GetOvertimeEvent_RPC(double sendMoment, int spawnsSeed)
    {
        //On met a jour le temps restant avant l'engagement
        //En prenant en compte le temps de trajet du message
        GameManager.timeLeftForKickoff = (float) (3 - (PhotonNetwork.Time - sendMoment));
        
        Ball.Hide();
        GameManager.timeLeft = 0;
        Spawns.randomSeed = spawnsSeed;

        //Au cas ou il y aurait une migration d'hote
        GameManagerHost.inOvertime = true;
        
        GameManager.script.RespawnAll();
    }
    
    //Cette methode est appellee sur le host si il y a un overtime
    public static void SendEndGameEvent(Team losingTeam)
    {
        pv.RPC("GetEndGameEvent_RPC", RpcTarget.Others, EndGameEvent(losingTeam));
    }

    private static object[] EndGameEvent(Team losingTeam)
    {
        return new object[]
        {
            (int) losingTeam
        };
    }

    [PunRPC]
    //Cette methode est appellee sur les clients au moment de la fin de partie (overtime ou pas)
    public void GetEndGameEvent_RPC(int losingTeam)
    {
        //Informe le game manager que la partie est terminee
        GameManager.EndGame((Team) losingTeam);
    }
}
