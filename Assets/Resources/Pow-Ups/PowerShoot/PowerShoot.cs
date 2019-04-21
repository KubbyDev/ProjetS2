using UnityEngine;

public class PowerShoot : MonoBehaviour
{
    public void Use_PowerShoot()
    {
        GetComponent<BallManager>().Use_PowerShoot();
    } 
}