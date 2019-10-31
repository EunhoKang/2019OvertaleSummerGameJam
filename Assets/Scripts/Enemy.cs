using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

	public float health = 100f;

	public GameObject deathEffect;

    

	public void TakeDamage (float damage)
	{
		health -= damage;
		if (health <= 0)
		{
			Die();
		}
	}

	void Die ()
	{
        GameObject effect=Instantiate(deathEffect, transform.position, Quaternion.identity);
        MapManager.mapmanager.enemySpawn();
		Destroy(gameObject);
        Destroy(effect, 1f);
	}
}
