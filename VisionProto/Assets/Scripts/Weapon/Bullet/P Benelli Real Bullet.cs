using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class PBenelliRealBullet : MonoBehaviour, IListener
{
    private Vector3 playerPosition;
    private float distance;

    // 샷건 거리관련 변수
    public float nearDistance = 10f;
    public float farDistance = 50f;

    // 샷건 데미지
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

        // 거리에 따라 데미지가 들어가는데 어떻게 해야할까?
        // Event로 처리해야 할 것 같은데..
        if (damageable != null)
        {
            EnemyController enemyController = collider.gameObject.GetComponent<EnemyController>();

            if (enemyController != null)
            {
                if (!enemyController.istestDetected)
                    return;
            }
            // Player 거리를 받아오고 PshotgunBullet 까지 거리를 계산한다.
            if (collider.CompareTag("NPC") || collider.CompareTag("EHead"))
            {
                if (playerPosition != Vector3.zero)
                    distance = Vector3.Distance(playerPosition, this.transform.position);
                else return;

                // 거리 비례
                if (distance < nearDistance)
                    damageable.Damaged(nearDamage, transform.position, transform.position, this.gameObject);
                else if (nearDistance <= distance && distance < farDistance)
                    damageable.Damaged(midDamage, transform.position, transform.position, this.gameObject);
                else if (farDistance <= distance)
                    damageable.Damaged(farDamage, transform.position, transform.position, this.gameObject);

            }
        }

        ///디텍터때랑 똑같은 문제가 발생할 것이다. 알아서 해라 ㅅㄱ 
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
        // 10개가 들어와야 하는데 5개만 들어오네?
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
