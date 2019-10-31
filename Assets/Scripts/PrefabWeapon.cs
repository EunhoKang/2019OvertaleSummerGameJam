using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabWeapon : MonoBehaviour {

    public Transform firePoint;
	public GameObject bulletPrefab;
    GameObject bullet;
    
    Vector3 mouseposition;

    public void Shoot ()
	{
		bullet=Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Destroy(bullet, 2f);
	}

    
}
