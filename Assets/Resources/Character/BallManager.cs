using System.Collections;
using UnityEngine;

//Ce script gere les interaction entre le joueur et la balle

public class BallManager : MonoBehaviour
{
    [SerializeField] [UnityEngine.Range(0, 10)] private float launchStrength = 2;        //La force avec laquelle la balle est jetee
    [SerializeField] [UnityEngine.Range(0, 5)] private float catchCooldown = 1;          //Le temps entre 2 tentative pour attraper la balle
    [SerializeField] [UnityEngine.Range(0, 5)] private float catchWidth = 1;             //L'imprecision autorisee pour attraper la balle
    [SerializeField] [UnityEngine.Range(1, 10)] private float PowerShootMultiplier = 5;  //La puissance du powershooot
    [SerializeField] private float PowerShootCooldown = 5;                               //Le temps pendant lequel le joueur peur utiliser powershoot    
    [HideInInspector] public bool hasBall = false;                        //Si le joueur a la balle
    
    private PlayerInfo infos;                                             //Le script qui contient les infos sur le joueur
    private float catchTimeLeft = 0;                                      //Le temps restant avant de pouvoir reutiliser le catch
    private float PowerShootTimeLeft = 0;                                 //Le temps restant pour utiliser le powershoot
    
    void Start()
    {
        infos = GetComponent<PlayerInfo>();
    }

    //Recuperation de la balle
    public void Catch()
    {
        if (catchTimeLeft > 0) 
            return;
        
        //On regarde si la balle est devant la camera a une distance inferieure a maxCatchDistance
        foreach (RaycastHit hit in Physics.SphereCastAll(infos.cameraPosition, catchWidth, infos.cameraRotation*Vector3.forward, infos.maxCatchRange))
            //On recupere la balle si on la touche ou si on touche son porteur
            if (hit.collider.CompareTag("Ball") && Ball.script.canBeCaught || hit.collider.CompareTag("Player") && hit.collider.gameObject.GetComponent<BallManager>().hasBall)
                //On enleve la possession de balle sur tous les joueurs et
                //on la donne au joueur qui vient de la recuperer
                Ball.UpdatePossessor(this.gameObject);

        catchTimeLeft = catchCooldown;
    }
    
    void Update()
    {
        infos.hasBall = hasBall;

        if (catchTimeLeft > 0)                 //Met a jour le temps restant pour attraper la balle
            catchTimeLeft -= Time.deltaTime;
        if (hasBall)                           //Si le joueur a la balle, on met son cooldown au max
            catchTimeLeft = catchCooldown;     //pour qu'il ne puisse pas la recuperer instant quand on lui prend
        
        if (PowerShootTimeLeft > 0)            //Met a jour le temps restant pour utiliser le powershoot
            PowerShootTimeLeft -= Time.deltaTime;
        
    }

    //Lance la balle devant lui
    public void Shoot()
    {
        if (hasBall)
            Ball.script.Shoot(infos.cameraRotation * Vector3.forward * launchStrength * 1000 * (PowerShootTimeLeft>0 ? PowerShootMultiplier : 1));
    }                                        //Si le joueur peut utiliser powershoot, on applique le multiplieur, sinon non

    public void Use_PowerShoot()
    {
        PowerShootTimeLeft = PowerShootCooldown; //Indique au script que le joueur a utilise le powerup powershoot
    }
}
