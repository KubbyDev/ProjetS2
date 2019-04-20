using Photon.Pun;
using UnityEngine;

public class IASetup : MonoBehaviour
{
    void Start()
    {
        //Donne un nom random a l'IA
        GetComponent<PlayerInfo>().nickname = RandomName.GenerateAI();
    }
}
