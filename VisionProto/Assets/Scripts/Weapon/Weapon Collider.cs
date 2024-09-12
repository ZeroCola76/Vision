using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static EventManager;

public class WeaponCollider : MonoBehaviour, IListener
{
    public bool isEquipped;
    public bool isthrowing;
    

    bool isDetected;

    public int throwingHeadDamage = 50;
    public int throwingDamage = 30;

    public BoxCollider boxCollider;

    private void Start()
    {
        isEquipped = true;
        isthrowing = false;
        EventManager.Instance.AddEvent(EventType.detected, OnEvent);
        boxCollider = GetComponent<BoxCollider>();

    }


    private void OnCollisionEnter(Collision collision)
    {
        if ((collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Floor")) && isthrowing)
        {
            EventManager.Instance.NotifyEvent(EventType.Throwing, false);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (isEquipped)
            return;


        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();

        Debug.Log(collision.gameObject.name);

        if (damageable != null)
        {
            EnemyController enemyController = collision.gameObject.GetComponent<EnemyController>();

            if (enemyController != null)
            {
                if (!enemyController.istestDetected)
                    return;
            }
            if (collision.gameObject.CompareTag("EHead"))
            {
                // 헤드 데미지
                damageable.Damaged(throwingHeadDamage, transform.position, transform.position, this.gameObject);
                gameObject.SetActive(false);
            }
            else if (collision.gameObject.CompareTag("NPC"))
            {
                // 일반 데미지
                damageable.Damaged(throwingDamage, transform.position, transform.position, this.gameObject);
                gameObject.SetActive(false);
            }
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
