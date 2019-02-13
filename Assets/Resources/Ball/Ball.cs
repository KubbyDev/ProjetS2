using UnityEngine;

public class Ball : MonoBehaviour
{
    public bool canBeCaught = true;          //Vrai si la balle peut etre recuperee

    public void FreezeBall()
    {
        canBeCaught = false;
    }

    public void DeFreezeBall()
    {
        canBeCaught = true;
    }
}
