using Photon.Pun;
using UnityEngine;

public class PlayerSetup : MonoBehaviour {

    //La liste des components qui seront desactives sur tous les joueurs non controlles par le joueur local
    [SerializeField] private Behaviour[] componentsToDisable; 

    void Start()
    {
        //Si le joueur qui vient de spawn est celui du client on laisse tout active
        if (GetComponent<PhotonView>().IsMine)
            return;

        //Sinon on desactive les components qui doivent l'etre
        foreach (Behaviour comp in componentsToDisable)
            comp.enabled = false;
    }
}
