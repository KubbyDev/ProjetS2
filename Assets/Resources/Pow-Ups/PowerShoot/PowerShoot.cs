using UnityEngine;

public class PowerShoot : MonoBehaviour
{
    public void Has_PowerShoot()
    {
        GetComponent<BallManager>().Use_PowerShoot();
    } 
}