using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PlayerSetup : MonoBehaviour, IPunInstantiateMagicCallback {

    //La liste des components qui seront desactives sur tous les joueurs non controlles par le joueur local
    [SerializeField] private Behaviour[] componentsToDisable; 

    void Awake()
    {
        //On set la team du joueur en fonction des nombres de joueurs dans les autres teams
        int blue = 0;
        int orange = 0;
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
            if (player.GetComponent<PlayerInfo>().team == Team.Blue)
                blue++;
            else
                orange++;
        if(blue == orange)
            GetComponent<PlayerInfo>().SetTeam(Teams.Random());
        else
            GetComponent<PlayerInfo>().SetTeam(blue > orange ? Team.Orange : Team.Blue);
        
        //Si le joueur qui vient de spawn est celui du client on laisse tout active, mais on le renseigne dans le PlayerInfo
        if (GetComponent<PhotonView>().IsMine)
            PlayerInfo.localPlayer = this.gameObject;
        else
            //Sinon on desactive les components qui doivent l'etre
            foreach (Behaviour comp in componentsToDisable)
                comp.enabled = false;
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        //Permet d'acceder au GameObject du joueur depuis un PhotonPlayer
        info.Sender.TagObject = this.gameObject;
    }
}
