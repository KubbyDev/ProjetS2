using System.Collections;
using UnityEngine;

public class Hook : MonoBehaviour
{
        [SerializeField] private GameObject hookObject;

        private bool Player_Has_Hook = true;

        private PlayerInfo playerinfo;

        private void Start()
        {
                playerinfo = GetComponent<PlayerInfo>();
        }

        public void Player_Got_Hook()
        {
                Player_Has_Hook = true;
        }
        
        public void Use_Hook()
        {
                if (Player_Has_Hook)
                {
                        GameObject hook = Instantiate(hookObject, transform.position + new Vector3(0, 1.5f, 0), Quaternion.identity);
                        Destroy(hook,3);
                        hook.GetComponent<HookBall>().UpdateDirection(gameObject, playerinfo.cameraAnchor.forward);
                }
        }
}