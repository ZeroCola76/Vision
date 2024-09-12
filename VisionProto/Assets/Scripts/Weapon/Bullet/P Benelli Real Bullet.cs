using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class PBenelliRealBullet : MonoBehaviour, IListener
{
    private Vector3 playerPosition;
    private float distance;

    // ���� �Ÿ����� ����
    public float nearDistance = 10f;
    public float farDistance = 50f;

    // ���� ������
    public int nearDamage = 50;
    public int midDamage = 30;
    public int farDamage = 10;

    bool isDetected;
    bool isShoot;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.Instance.AddEvent(EventType.detected, OnEvent);
        EventManager.Instance.AddEvent(EventType.PlayerPosition, OnEvent);
    }

    private void OnTriggerEnter(Collider collider)
    {
        IDamageable damageable = collider.gameObject.GetComponent<IDamageable>();

        // �Ÿ��� ���� �������� ���µ� ��� �ؾ��ұ�?
        // Event�� ó���ؾ� �� �� ������..
        if (damageable != null)
        {
            EnemyController enemyController = collider.gameObject.GetComponent<EnemyController>();

            if (enemyController != null)
            {
                if (!enemyController.istestDetected)
                    return;
            }
            // Player �Ÿ��� �޾ƿ��� PshotgunBullet ���� �Ÿ��� ����Ѵ�.
            if (collider.CompareTag("NPC") || collider.CompareTag("EHead"))
            {
                if (playerPosition != Vector3.zero)
                    distance = Vector3.Distance(playerPosition, this.transform.position);
                else return;

                // �Ÿ� ���
                if (distance < nearDistance)
                    damageable.Damaged(nearDamage, transform.position, transform.position, this.gameObject);
                else if (nearDistance <= distance && distance < farDistance)
                    damageable.Damaged(midDamage, transform.position, transform.position, this.gameObject);
                else if (farDistance <= distance)
                    damageable.Damaged(farDamage, transform.position, transform.position, this.gameObject);

            }
        }

        ///�����Ͷ��� �Ȱ��� ������ �߻��� ���̴�. �˾Ƽ� �ض� ���� 
        if (collider.CompareTag("NPC") || collider.CompareTag("EHead"))
        {
            //EventManager.Instance.NotifyEvent(EventType.NPCHit, isShoot = true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("NPC") || other.CompareTag("EHead"))
        {
            //EventManager.Instance.NotifyEvent(EventType.NPCHit, isShoot = false);
        }
    }

    private void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.tag == "Wall" || collider.gameObject.tag == "Floor")
        {
            //PoolManager.Instance.ReturnToPool(this.gameObject, "PImageBenelli828u");
            gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 10���� ���;� �ϴµ� 5���� ������?
        if (collision.gameObject.tag == "Wall" || collision.gameObject.tag == "Floor")
        {
            //PoolManager.Instance.ReturnToPool(this.gameObject, "PImageBenelli828u");
            gameObject.SetActive(false);
        }
    }

    public void OnEvent(EventType eventType, object param = null)
    {
        switch (eventType)
        {
            case EventType.PlayerPosition:
                {
                    playerPosition = (Vector3)param;
                }
                break;
            case EventType.detected:
                {
                    isDetected = (bool)param;
                }
                break;
        }
    }

}
