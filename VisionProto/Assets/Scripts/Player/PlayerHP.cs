using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class PlayerHP : MonoBehaviour, IDamageable
{
    public int HP;
    public int currentHP;


    void Start()
    {
        currentHP = HP;
        EventManager.Instance.NotifyEvent(EventType.PlayerHPUI, currentHP);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            currentHP -= 10;
            EventManager.Instance.NotifyEvent(EventType.PlayerHPUI, currentHP);
        }
    }

    public void Damaged(int damage, Vector3 hitPoint, Vector3 hitNormal, GameObject source)
    {
        currentHP -= damage;
        EventManager.Instance.NotifyEvent(EventType.PlayerHPUI, currentHP);
        //Debug.Log("currentHp : "+ currentHP);

        if(currentHP <= 0)
        {
            Died();
        }
    }

    public void Died()
    {
        Debug.Log("die");
    }
 
}
