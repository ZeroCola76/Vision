using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class PFNSCARBullet : MonoBehaviour, IListener
{
    bool isDetected;

    public int normalDamage = 10;
    public int headDamage = 30;

    void Start()
    {
        EventManager.Instance.AddEvent(EventType.detected, OnEvent);
        //Invoke("BulletFalse", 3f);
    }

    void BulletFalse()
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider collider)
    {
        IDamageable damageable = collider.gameObject.GetComponent<IDamageable>();

        if (damageable != null)
        {
            EnemyController enemyController = collider.gameObject.GetComponent<EnemyController>();

            if (enemyController != null)
            {
                if (!enemyController.istestDetected)
                    return;
            }

            if (collider.CompareTag("EHead"))
            {
                // ��弦 ó��
                damageable.Damaged(headDamage, transform.position, transform.position, this.gameObject);
                //PoolManager.Instance.ReturnToPool(this.gameObject, "PFNSCAR");
                //gameObject.SetActive(false);
            }
            else if (collider.CompareTag("NPC"))
            {
                // �Ϲ� ������ ó��
                damageable.Damaged(normalDamage, transform.position, transform.position, this.gameObject);
                //PoolManager.Instance.ReturnToPool(this.gameObject, "PFNSCAR");
                //gameObject.SetActive(false);
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Floor"))
        {
            ContactPoint contact = collision.contacts[0];
            Vector3 hitPoint = contact.point;
            Vector3 hitNormal = contact.normal;

            EffectManager.Instance.ExecutionEffect(Effect.BulletDecal, hitPoint, Quaternion.LookRotation(hitNormal), collision.transform);
            EffectManager.Instance.ExecutionEffect(Effect.GunHit, hitPoint, Quaternion.LookRotation(hitNormal), collision.transform);


            gameObject.SetActive(false);
        }
    }

    public void OnEvent(EventType eventType, object param = null)
    {
        switch (eventType)
        {
            case EventType.detected:
                {
                    isDetected = (bool)param;
                }
                break;
        }
    }

}
