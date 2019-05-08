using System;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    [SerializeField] private float scaleAnimDuration = 0.2f;
    [SerializeField] private float baseScale = 0.9f;
    [SerializeField] private float finalScale = 1.4f;
    
    private Transform cross;
    private Image circle;
    
    private PlayerInfo infos;
    private BallManager ballManager;

    private float animProgress;    //Si la balle etait attrapable a la frame precedente
    private bool init = false;     //Passe a true quand l'initialisation s'est faite

    //Initialisation. Il faut attendre que le PlayerInfo.localPlayer recoive le joueur local
    private void Init()
    {
        init = true;

        infos = PlayerInfo.localPlayer.GetComponent<PlayerInfo>();
        ballManager = PlayerInfo.localPlayer.GetComponent<BallManager>();

        circle = GetComponent<Image>();
        cross = transform.GetChild(0);
    }

    void Update()
    {
        //On initialise si le PlayerInfo est pret
        if (!init && PlayerInfo.localPlayer != null)
            Init();

        //On ne fait rien tant que l'init n'est pas faite
        if (!init)
            return;
        
        //Affichage du cooldown pour attraper la balle
        if (ballManager.catchTimeLeft > 0)
        {
            circle.fillAmount = (1 - ballManager.catchTimeLeft) / ballManager.catchCooldown;
            Tools.ModifyAlpha(0.5f, circle);
        }
        else
        {
            circle.fillAmount = 1;
            Tools.ModifyAlpha(0.1f, circle);
        }

        //Animation de la croix centrale
        if (CanCatchBall())
            animProgress += Time.deltaTime;
        else
            animProgress -= Time.deltaTime;

        animProgress = Math.Max(0, Math.Min(animProgress, scaleAnimDuration));
        ScaleCross(animProgress / scaleAnimDuration);
    }

    private bool CanCatchBall()
    {
        if (ballManager.catchTimeLeft > 0 || !Ball.script.canBeCaught) 
            return false;
        
        //On regarde si la balle est devant la camera a une distance inferieure a maxCatchDistance
        foreach (RaycastHit hit in Physics.SphereCastAll(
            infos.cameraPosition, ballManager.catchWidth, infos.cameraRotation * Vector3.forward, infos.maxCatchRange))
        {
            if (hit.collider.CompareTag("Ball") || 
                hit.collider.CompareTag("Player") && hit.collider.gameObject.GetComponent<BallManager>().hasBall)
                return true;   
        }

        return false;
    }

    //0: scale de base, 1: scale quand la balle est attrapable
    private void ScaleCross(float _animProgress)
    {
        cross.localScale = Vector3.one *(_animProgress*(finalScale-baseScale) + baseScale);
    }
}