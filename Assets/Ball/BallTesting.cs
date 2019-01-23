using UnityEngine;
using UnityEngine.Networking;

public class BallTesting : NetworkBehaviour
{ 
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            CmdAddForceBall(Camera.main.transform.forward * 1000);
    }

    [Command]
    void CmdAddForceBall(Vector3 force)
    {
        GetComponent<Rigidbody>().AddForce(force);
    }
}
