using Photon.Pun;
using UnityEngine;

public class Back : MonoBehaviour
{    
    public bool Player_Has_Back = false;

    private PhotonView pv;
    [SerializeField] private GameObject theVoidPrefab;

    void Awake()
    {
        pv = GetComponent<PhotonView>();
    }
    
    public void Player_Got_Back()
    {
        Player_Has_Back = true;
    }

    public void TP_Back()
    {
        if (Player_Has_Back)
        {
            pv.RPC("SpawnBackParticle_RPC", RpcTarget.All, transform.position);
            
            Spawns.AtRandomUnused(this.gameObject);
            Player_Has_Back = false;
        }
    }

    [PunRPC]
    public void SpawnBackParticle_RPC(Vector3 position)
    {
        GameObject particle = Instantiate(theVoidPrefab, position, Quaternion.identity);
        ParticleSystem.MainModule main = particle.GetComponent<ParticleSystem>().main;
        main.startColor = new ParticleSystem.MinMaxGradient(GetComponent<PlayerInfo>().team.GetMaterial().color);
    }
}

