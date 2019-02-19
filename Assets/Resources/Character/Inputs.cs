using UnityEngine;

public class Inputs : MonoBehaviour
{
    //Ce script sert a contenir les inputs choisis par le joueur
    public KeyCode[] inputs;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
