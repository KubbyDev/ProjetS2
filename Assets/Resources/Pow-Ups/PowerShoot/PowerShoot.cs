using UnityEngine;

public class PowerShoot : MonoBehaviour
{
    public const float multiplier = 1.2f;   //La puissance du powershooot
    public const float cooldown = 3;        //Le temps pendant lequel le joueur peur utiliser powershoot    
    
    public bool Player_Has_PowerShoot = false;
    
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