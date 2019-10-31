using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{

    public float m_speed = 50f;
    public int m_damage = 40;
    public Rigidbody2D rib;
    public GameObject m_impactEffect;
    public float m_ExplosionRadius = 1.4f;
    public float m_MaxDamage = 100f;
    public float m_MaxForce = 1.4f;

    // Use this for initialization
    void Start()
    {
        rib.velocity = transform.right * m_speed;
    }
    
    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.gameObject.tag != "isGothrough")
        {
            GameObject impact = Instantiate(m_impactEffect, transform.position, transform.rotation);
            Destroy(impact, 1f);
            Destroy(gameObject);
        }
        else
        {
            return;
        }

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, m_ExplosionRadius, ~((1<<LayerMask.NameToLayer("Default"))+(1 << LayerMask.NameToLayer("VertPlatform"))));
        for (int i = 0; i < colliders.Length; i++)
        {
            Debug.Log(colliders[i]);
            TrapDoor trapdoor = colliders[i].GetComponent<TrapDoor>();
            if (trapdoor != null)
            {
                Debug.Log(0);
                trapdoor.TakeDamage(100);
            }
            //플레이어 맞추면 콜리더 두개라 두번 반응. 후에 조치 필요
            Rigidbody2D targetRigidbody = colliders[i].GetComponent<Rigidbody2D>();
            if (!targetRigidbody)
                continue;

            targetRigidbody.AddForce(new Vector2(targetRigidbody.position.x-transform.position.x, targetRigidbody.position.x - transform.position.x)*m_MaxForce);

            Enemy enemy = colliders[i].GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(CalculateDamage(targetRigidbody.position));
            }

            
        }

    }
    
    private float CalculateDamage(Vector3 targetPosition)
    {
        Debug.Log(targetPosition);
        Vector3 explosionToTarget = targetPosition - transform.position;
        float explosionDistance = explosionToTarget.magnitude;
        float relativeDistance = (2f*m_ExplosionRadius - explosionDistance) / 2f*m_ExplosionRadius;
        float damage = Mathf.Abs(relativeDistance * m_MaxDamage);

        damage = Mathf.Max(0f, damage);

        return damage;
    }
}