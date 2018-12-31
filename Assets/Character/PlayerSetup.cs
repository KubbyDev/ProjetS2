using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerSetup : NetworkBehaviour {

    [SerializeField] Behaviour[] componentsToKeep;

    void Start()
    {
        //Si le joueur qui vient de spawn est celui du client on laisse tout active
        if (isLocalPlayer)
            return;

        //Sinon on desactive les components qui doivent l'etre
        List<Behaviour> scripts = new List<Behaviour>(GetComponents<Behaviour>());

        Debug.Log(scripts.Count);

        foreach (Behaviour comp in scripts)
            if ( ! Array.Exists(componentsToKeep, e => e == comp))
                comp.enabled = false;
    }
}
