using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class VPRenderFeature : MonoBehaviour, IListener
{
    private bool isVPState;

    public RenderObjects vpState;
    public RenderObjects vpStateThinOutline;
    public RenderObjects vpStateThinkOutline;
    public RenderObjects npcStencil;
    public FullScreenPassRendererFeature fullScreenOutline;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.Instance.AddEvent(EventType.VPState, OnEvent);
    }

    private void SwitchRenderFeature(bool _isOn)
    {
        vpState.SetActive(_isOn);
        vpStateThinOutline.SetActive(_isOn);
        vpStateThinkOutline.SetActive(_isOn);
        npcStencil.SetActive(_isOn);
        fullScreenOutline.SetActive(_isOn);
    }

    public void OnEvent(EventType eventType, object param = null)
    {
        switch (eventType)
        {
            case EventType.VPState:
                {
                    isVPState = (bool)param;

                    // Fade In / Fade Out Effect를 넣어야 한다.

                    if (isVPState)
                        SwitchRenderFeature(true);
                    else
                        SwitchRenderFeature(false);
                }
                break;
        }
    }
}
