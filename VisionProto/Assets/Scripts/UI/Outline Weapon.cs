using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OutlineWeapon : MonoBehaviour, IListener
{
    // ���콺�� ����� ��
    public bool isMouseEnter;
    private int outlineLayer;
    private int gunLayer;
    public bool isDone;
    private bool isVPState;

    // Start is called before the first frame update
    void Start()
    {
        int outlineLayerValue = LayerMask.GetMask("Outline");
        int gunLayerValue = LayerMask.GetMask("Gun");
        isDone = false;
        outlineLayer = (int)(Mathf.Log(outlineLayerValue) / Mathf.Log(2));
        gunLayer = (int)(Mathf.Log(gunLayerValue) / Mathf.Log(2));
        EventManager.Instance.AddEvent(EventType.VPState, OnEvent);
    }

    public void GunLayer()
    {
        this.gameObject.layer = gunLayer;
    }

    private void OnMouseEnter()
    {
        if (isVPState)
            return;

        // ���콺�� ������� layer�� Outline���� �ٲ�� �Ѵ�.
        if (!isDone)
        {
            isMouseEnter = true;
            this.gameObject.layer = outlineLayer;
        }
    }

    private void OnMouseOver()
    {
        if(isVPState)
        {
            this.gameObject.layer = gunLayer;
        }
    }

    private void OnMouseExit()
    {
        // ���콺�� ���� �ʾ����� layer�� �ٽ� Gun���� ������.
        if (!isDone)
        {
            isMouseEnter = false;
            this.gameObject.layer = gunLayer;
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
