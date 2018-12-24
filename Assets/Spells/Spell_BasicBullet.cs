using UnityEngine;
using UnityEngine.Networking;

public class Spell_BasicBullet : NetworkBehaviour {

	public GameObject bulletPrefab;
    public Transform bulletSpawn;

    public void Fire()
    {
        CmdFire();
    }
    
	[Command]
    void CmdFire()
    {
        // Create the Bullet from the Bullet Prefab
        var bullet = Instantiate (
            bulletPrefab,
            bulletSpawn.position,
            bulletSpawn.rotation);

        // Add velocity to the bullet
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 6;

        NetworkServer.Spawn(bullet);

        // Destroy the bullet after 2 seconds
        Destroy(bullet, 2.0f);
    }
}
