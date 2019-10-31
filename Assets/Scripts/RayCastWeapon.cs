using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class RayCastWeapon : MonoBehaviour {

    public Transform firePoint;

    public Transform flip;
    public int damage = 40;
	public GameObject impactEffect;
	public LineRenderer lineRenderer;

    Vector3 mouseposition;
    GameObject bullet;

    // Update is called once per frame
    void Update () {

        mouseposition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetButtonDown("Fire1"))
        {
            StartCoroutine(Shoot(mouseposition));
        }

	}

    IEnumerator Shoot (Vector3 mp)
	{
        LayerMask mask = (1 << LayerMask.NameToLayer("VertPlatform"))+ (1 << LayerMask.NameToLayer("Player"));
        mask = ~mask;


        RaycastHit2D hitInfo = Physics2D.Raycast(firePoint.position, mp-firePoint.position, Mathf.Infinity,mask);
		if (hitInfo)
		{
			Enemy enemy = hitInfo.transform.GetComponent<Enemy>();
			if (enemy != null)
			{
				enemy.TakeDamage(damage);
			}


			bullet=Instantiate(impactEffect, hitInfo.point, Quaternion.identity);

			lineRenderer.SetPosition(0, firePoint.position);
			lineRenderer.SetPosition(1, hitInfo.point);
		} else
		{
			lineRenderer.SetPosition(0, firePoint.position);
			lineRenderer.SetPosition(1, mp+ 100*(mp - firePoint.position));
		}

		lineRenderer.enabled = true;

		yield return 0;

		lineRenderer.enabled = false;
        Destroy(bullet,0.5f);
	}
}
