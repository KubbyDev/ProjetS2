using Photon.Pun;
using UnityEngine;

public class Hook : MonoBehaviour
{
        public const float lifeTime = 1f;
        public const int networkIdentifier = 1;
        
        [SerializeField] private GameObject hookObject;

        public bool Player_Has_Hook = false;

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
                
                Vector3 position = transform.position + new Vector3(0,1,0) + transform.forward * 1.0f;
                Vector3 direction = playerinfo.cameraRotation * Vector3.forward;
                        
                //Informe les autres clients
                pv.RPC("SpawnHook", RpcTarget.Others, position, direction);

                GameObject hook = Instantiate(hookObject, position, Quaternion.identity);
                
                ParticleSystem.MainModule main = hook.transform.Find("ElectricParticles").GetComponent<ParticleSystem>().main;
                main.duration = lifeTime;
                main.startColor = GetComponent<PlayerInfo>().team.GetMaterial().color;
                hook.transform.Find("ElectricParticles").GetComponent<ParticleSystem>().Play();

                //Met en place le composant qui gere le projectile sur le reseau
                PhotonView view = hook.AddComponent<PhotonView>();
                view.ViewID = pv.ViewID + networkIdentifier;

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
                
                //Met en place le composant qui gere le projectile sur le reseau
                PhotonView view = hook.AddComponent<PhotonView>();
                view.ViewID = pv.ViewID + networkIdentifier;
                
                Destroy(hook, lifeTime - latency);
                hook.GetComponent<HookBall>().UpdateDirection(this.gameObject, direction, false);
                
                ParticleSystem.MainModule main = hook.transform.Find("ElectricParticles").GetComponent<ParticleSystem>().main;
                main.duration = lifeTime;
                main.startColor = GetComponent<PlayerInfo>().team.GetMaterial().color;
                hook.transform.Find("ElectricParticles").GetComponent<ParticleSystem>().Play();
        }
}