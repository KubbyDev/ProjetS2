using Photon.Pun;
using System.Collections;
using UnityEngine;

public class BasicSpell : MonoBehaviour
{
    [SerializeField] private GameObject BasicSpellbullet;

    private bool BasicSpellOffCooldown = true;
    private float BasicSpellCooldown = 15f;


    private PlayerInfo playerinfocaster;

    void Start()
    {
        playerinfocaster = GetComponent<PlayerInfo>();
    }


    public void Basic_Spell()
    {
        if (BasicSpellOffCooldown)
            BasicSpellCoroutine();
    }

    IEnumerator BasicSpellCoroutine()
    {
        GameObject bullet = PhotonNetwork.Instantiate("Spells/BasicSpell/BasicSpellBullet",
                    transform.position + new Vector3(0, 1.5f, 0),
                    Quaternion.identity);
        Destroy(bullet, 3);
        bullet.GetComponent<HookBall>().UpdateDirection(bullet, playerinfocaster.cameraAnchor.forward);
        
        BasicSpellOffCooldown = false;
        yield return new WaitForSeconds(BasicSpellCooldown);
        BasicSpellOffCooldown = true;

    }

}