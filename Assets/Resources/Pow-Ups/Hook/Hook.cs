using Photon.Pun;
using UnityEngine;

public class Hook : MonoBehaviour
{
        [SerializeField] private GameObject hookObject;
        [SerializeField] private float lifeTime = 3.0f;

        [SerializeField] private bool Player_Has_Hook = false;

        private PhotonView pv;
        private PlayerInfo playerinfo;

        private void Start()
        {
                playerinfo = GetComponent<PlayerInfo>();
                pv = GetComponent<PhotonView>();
        }

        public void Player_Got_Hook()
        {
                Player_Has_Hook = true;
        }
        
        public void Use_Hook()
        {
                if (!Player_Has_Hook) 
                        return;
                
                Vector3 position = transform.position + new Vector3(0, 0.5f, 0) + transform.forward * 1.0f;
                Vector3 direction = playerinfo.cameraAnchor.forward;
                        
                GameObject hook = Instantiate(hookObject, position, Quaternion.identity);
                        
                //Informe les autres clients
                pv.RPC("SpawnHook", RpcTarget.Others, position, direction);   
                        
                Destroy(hook, lifeTime);
                hook.GetComponent<HookBall>().UpdateDirection(this.gameObject, direction, true);
                Player_Has_Hook = false;
        }

        [PunRPC]
        public void SpawnHook(Vector3 position, Vector3 direction, PhotonMessageInfo info)
        {
                float latency = Tools.GetLatency(info.timestamp);
        
                GameObject hook = Instantiate(hookObject,
                        position + latency*direction,
                        Quaternion.identity);
                
                Destroy(hook, lifeTime - latency);
                hook.GetComponent<HookBall>().UpdateDirection(this.gameObject, direction, false);
        }
}