using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSetup : MonoBehaviour {

    [SerializeField] private Behaviour[] scriptsToKeep;
    [SerializeField] private Behaviour[] componentsToDisable;

    void Start()
    {
        //Si le joueur qui vient de spawn est celui du client on laisse tout active
        if (GetComponent<PhotonView>().IsMine)
            return;

        //Sinon on desactive les components qui doivent l'etre
        foreach (Behaviour comp in componentsToDisable)
            comp.enabled = false;

        List<Behaviour> scripts = new List<Behaviour>(GetComponents<Behaviour>());
        foreach (Behaviour script in scripts)
            if ( ! Array.Exists(scriptsToKeep, e => e == script))
                script.enabled = false;
    }
}
