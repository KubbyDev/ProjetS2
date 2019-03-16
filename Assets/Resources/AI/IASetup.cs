using Photon.Pun;
using UnityEngine;

public class IASetup : MonoBehaviour
{
    void Start()
    {
        //Permet de donner la priorite au host concernant les IA
        //Chaque client calcul les deplacement des IA de son cote, mais
        //Si les calculs different, ceux du host sont prioritaires
        if(PhotonNetwork.IsMasterClient)
            GetComponent<PhotonView>().RequestOwnership();

        //Donne un nom random a l'IA
        GetComponent<PlayerInfo>().nickname = RandomName.GenerateAI();
    }
}
