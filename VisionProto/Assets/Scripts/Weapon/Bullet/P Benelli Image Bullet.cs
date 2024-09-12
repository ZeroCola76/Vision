using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PBenelliImageBullet : MonoBehaviour
{
    // Decal�� ������ �� �ʿ��� ��?
    // �Ѿ��� Rotation���� ������?

    

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Floor"))
        {
            //PoolManager.Instance.ReturnToPool(this.gameObject, "PImageBenelli828u");

            ContactPoint contact = collision.contacts[0];
            Vector3 hitPoint = contact.point;
            Vector3 hitNormal = contact.normal;

            EffectManager.Instance.ExecutionEffect(Effect.BulletDecal, hitPoint, Quaternion.LookRotation(hitNormal), collision.transform);
            EffectManager.Instance.ExecutionEffect(Effect.GunHit, hitPoint, Quaternion.LookRotation(hitNormal), collision.transform);

            gameObject.SetActive(false);
        }
    }
}
