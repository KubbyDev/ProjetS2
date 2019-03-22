using UnityEngine;

public class PowerShoot : MonoBehaviour
{
    public void Has_PowerShoot()
    {
        GetComponent<BallManager>().Has_PowerShoot();
    } 
}