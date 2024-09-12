using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// VP 상태일때 VP
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
            // Pickup 상태이면 notify Event로 change 해야 하는데 Auto Pickup도 꺼야함
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
