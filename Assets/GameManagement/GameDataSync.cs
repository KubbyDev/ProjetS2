using Photon.Pun;
using Photon.Realtime;
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
        //On met a jour le temps restant avant le debut de la game 
        //En prenant en compte le temps de trajet du message
        PreGameManager.timeLeftToStart =  (float) (time - (PhotonNetwork.Time - sendMoment));
        
        //La seed du LCG des Spawns (Pour que tous les clients aient les memes nombres random)
        Spawns.randomSeed = spawnsSeed;
        
        //On informe le GameManager que le premier packet est arrive
        GameManager.OnFirstPacketRecieved();
    }
    
    // Packet de but ---------------------------------------------------------------------------------------------------

    //Cette methode est appellee sur le host quand il y a un but (pour confirmer le but aux clients)
    public static void SendOnGoalData(bool isBlue)
    {
        pv.RPC("GetOnGoalData_RPC", RpcTarget.Others, OnGoalData(isBlue));
    }

    private static object[] OnGoalData(bool isBlue)
    {
        return new object[]
        {
            isBlue,                          //True: Le but est dans le but bleu (donc marque par les oranges)
            Ball.ball.transform.position,    //La position de la balle quand elle est entree dans le but
            PhotonNetwork.Time,              //Le moment auquel le message part (pour prendre en compte le temps de trajet)
            GameManager.timeLeft,            //Le temps restant avant la fin de la partie
            GameManager.timeLeftForKickoff,  //Le temps restant avant que les joueurs puissent bouger
            Spawns.randomSeed                //La seed du LCG des Spawns (Pour que tous les clients aient les memes nombres random)
        };
    }
    
    [PunRPC]
    //Cette methode est appellee sur les clients au moment des buts pour les confirmer
    public void GetOnGoalData_RPC(bool isBlue, Vector3 ballPosition, double sendMoment, float pTimeLeft, float pTimeLeftToKickoff, int spawnsSeed)
    {
        //On met a jour le temps restant avant l'engagement
        //En prenant en compte le temps de trajet du message
        GameManager.timeLeftForKickoff = (float) (pTimeLeftToKickoff - (PhotonNetwork.Time - sendMoment));
        
        GameManager.timeLeft = pTimeLeft;
        Spawns.randomSeed = spawnsSeed;
        
        //On informe le GameManager qu'il y a eu un but
        GameManager.script.OnGoal(isBlue, ballPosition);
    }
    
    // Packet de fin de game -------------------------------------------------------------------------------------------
    
    //Cette methode est appellee sur le host quand la partie est terminee
    public static void SendEndGameEvent()
    {
        pv.RPC("GetEndGameEvent_RPC", RpcTarget.Others, EndGameEvent());
    }

    private static object[] EndGameEvent()
    {
        return new object[] {};
    }

    [PunRPC]
    //Cette methode est appellee sur les clients au moment de la fin de partie
    public void GetEndGameEvent_RPC()
    {
        GameManager.EndGame();
    }
}
