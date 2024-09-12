using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// VP �����϶� VP
/// </summary>
public class WeaponSelect : MonoBehaviour, IListener
{
    public GameObject dagger;
    public GameObject vpWeapon;
    public AutoPickup autoPickup;
    private bool isVPState;

    // Start is called before the first frame update
    void Start()
    {
        isVPState = false;
        EventManager.Instance.AddEvent(EventType.VPState, OnEvent);
    }

    // Update is called once per frame
    void Update()
    {
        if(isVPState)
        {
            dagger.SetActive(false);
            // Pickup �����̸� notify Event�� change �ؾ� �ϴµ� Auto Pickup�� ������
            autoPickup.isActive = false;
            autoPickup.GunListClear();
            EventManager.Instance.NotifyEvent(EventType.Change);
            vpWeapon.SetActive(true);
        }
        else
        {
            dagger.SetActive(true);
            vpWeapon.SetActive(false);
            autoPickup.isActive = true;
        }    
    }

    public void OnEvent(EventType eventType, object param = null)
    {
        switch (eventType) 
        {
            case EventType.VPState:
                {
                    isVPState = (bool)param;
                }
                break;
        }
    }
}
