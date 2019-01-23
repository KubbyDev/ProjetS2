using UnityEngine;
using UnityEngine.Networking;
using Photon.Pun;

public class BallTesting : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            RPC_AddForceBall(Camera.main.transform.forward * 1000);
    }

    [PunRPC]
    void RPC_AddForceBall(Vector3 force) 
    {
        GetComponent<Rigidbody>().AddForce(force);
    }
}
