using UnityEngine;

public class IASetup : MonoBehaviour
{
    void Start()
    {
        //Donne un nom random a l'IA
        transform.Find("Nickname").GetComponent<TextMesh>().text = GetComponent<PlayerInfo>().nickname = RandomName.GenerateAI();
    }
}
