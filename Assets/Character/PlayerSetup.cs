using UnityEngine;
using UnityEngine.Networking;

public class PlayerSetup : NetworkBehaviour {

    [SerializeField] Behaviour[] componentsToDisable;

    void Start()
    {
        if (!isLocalPlayer)
            disableComponents(componentsToDisable);
    }

    private void disableComponents(Behaviour[] compToDisable)
    {
        //Components of childs of the player
        foreach(Behaviour b in compToDisable)
            b.enabled = false;
    }

}
