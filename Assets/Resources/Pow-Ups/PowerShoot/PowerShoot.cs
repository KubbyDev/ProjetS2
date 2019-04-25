using UnityEngine;

public class PowerShoot : MonoBehaviour
{
    private bool Player_Has_PowerShoot = false;
    
    public void Player_Got_PowerShoot()
    {
        Player_Has_PowerShoot = true;
    }

    public void Use_PowerShoot()
    {
        if (Player_Has_PowerShoot)
        {
            GetComponent<BallManager>().Use_PowerShoot();
            Player_Has_PowerShoot = false;
        }
    } 
}