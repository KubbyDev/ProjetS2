using Photon.Pun;
using UnityEngine;

public class Warden : MonoBehaviour
{
    [SerializeField] private float Freeze1_Duration = 1f;  //Duree du freeze complet (pas de recuperation, pas de mouvement)
    
    //J'ai mis ces variables en commentaire parce qu'elle etaient pas utilisees
    //SerializeField] private float Freeze2_Duration = 4f;  //Duree du freeze partiel (pas de mouvement mais recuperation possible)
    //[SerializeField] private float Freeze_Cooldown = 20f;  //Duree du cooldown

    private bool Freeze_Cooldown = true;   //Vrai si le cooldown du freeze est termine
    [SerializeField] private GameObject FreezeBall;
    

    public void Freeze()
    {
        //cree et prend la position de la ball en fonction de la camera
        PhotonNetwork.Instantiate("Spells/Warden/FreezeBall", 
            transform.position + transform.forward, 
            GetComponent<PlayerInfo>().cameraAnchor.rotation);
        
    }

    
}
