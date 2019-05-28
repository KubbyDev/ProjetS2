using Photon.Pun;
using UnityEngine;

public class IASetup : MonoBehaviour
{
    public void Init()
    {
        //Donne un nom random a l'IA
        string nickname = RandomName.GenerateAI();
        GetComponent<PhotonView>().RPC("SetNickname_RPC", RpcTarget.All, nickname);
    }

    [PunRPC]
    public void SetNickname_RPC(string nickname)
    {
        transform.Find("Nickname").GetComponent<TextMesh>().text = GetComponent<PlayerInfo>().nickname = nickname;
    }
}
