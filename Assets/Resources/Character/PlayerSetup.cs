using Photon.Pun;
using UnityEngine;

//Cette classe initialise le joueur au moment ou il est instancie

public class PlayerSetup : MonoBehaviour, IPunInstantiateMagicCallback {

    //La liste des components qui seront desactives sur tous les joueurs non controlles par le joueur local
    [SerializeField] private Behaviour[] componentsToDisable; 

    void Awake()
    {
        //Si le joueur qui vient de spawn est celui du client on laisse tout active sauf le nickname
        if (GetComponent<PhotonView>().IsMine)
        {
            PlayerInfo.localPlayer = this.gameObject;
            transform.Find("Nickname").GetComponent<MeshRenderer>().enabled = false;

            CooldownDisplay.localPlayerInfos = this.gameObject.GetComponent<PlayerInfo>();
        }
        else
        {
            //Sinon on desactive les components qui doivent l'etre
            foreach (Behaviour comp in componentsToDisable)
                comp.enabled = false;   
        }
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        //Permet d'acceder au GameObject du joueur depuis un PhotonPlayer
        info.Sender.TagObject = this.gameObject;
        
        //On recupere le pseudo et le hero du joueur 
        transform.Find("Nickname").GetComponent<TextMesh>().text = GetComponent<PlayerInfo>().nickname = 
            info.Sender.NickName;
        PlayerInfo playerInfo = GetComponent<PlayerInfo>();
        playerInfo.SetHero(playerInfo.hero);
    }
}
