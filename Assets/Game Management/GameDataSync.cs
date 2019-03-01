using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class GameDataSync : MonoBehaviour
{
    private static PhotonView pv;       //Reference au script qui gere les echanges sur le reseau

    void Start()
    {
        pv = GetComponent<PhotonView>();
    }

    //Cette methode est appellee sur le Host quand un joueur rejoint la salle
    //Elle met a jour le GameData du joueur qui vient de rejoindre
    public static void SendFirstPacket(Player player)
    {
        pv.RPC("GetFirstPacket_RPC", player, GameData.SendFirstPacket());
    }

    [PunRPC]
    //Cette methode est appellee sur le joueur qui vient de rejoindre la salle, pour mettre a jour son GameData
    private static void GetFirstPacked_RPC(float time, int preset, int spawnsSeed)
    {
        GameData.ReceiveFirstPacket(time, (GamePreset) preset, spawnsSeed);
    }

    //Cette methode est appellee sur le host quand il y a un but (pour confirmer le but aux clients)
    public static void SendOnGoalData(bool isBlue)
    {
        pv.RPC("GetOnGoalData_RPC", RpcTarget.Others, GameData.SendOnGoalData(isBlue));
    }

    //Cette methode est appellee sur les clients au moment des buts pour les confirmer
    public static void GetOnGoalData_RPC(bool isBlue, Vector3 ballPosition)
    {
        GameData.ReceiveOnGoalData();
        GameManager.script.OnGoal(isBlue, ballPosition);
    }
}