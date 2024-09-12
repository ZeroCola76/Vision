using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AnimationInformation
{
    public string stateName;
    public int layer;
    public float normalizedTime;
}

public class PlayerAnimator : MonoBehaviour, IListener
{
    // ÀÌ°Å private 
    public Animator animator;

    private void Awake()
    {
        //animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        EventManager.Instance.AddEvent(EventType.PlayerAnimator, OnEvent);
    }

    public float Speed
    {
        set => animator.SetFloat("Speed", value);
        get => animator.GetFloat("Speed");
    }

    public float Idle
    {
        set => animator.SetFloat("Idle", value);
        get => animator.GetFloat("Idle");
    }

    public void OnReload()
    {
        animator.SetTrigger("OnReload");
    }
    public void OnGrappling()
    {
        animator.SetTrigger("OnGrappling");
    }

    public bool IsCurrentAnimation(string name)
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(name);
    }

    public void Play(string  stateName, int layer, float normalizedTime)
    {
        animator.Play(stateName, layer, normalizedTime);
    }

    public void OnEvent(EventType eventType, object param = null)
    {
        switch (eventType)
        {
            case EventType.PlayerAnimator:
                {
                    AnimationInformation info = (AnimationInformation)param;
                    Play(info.stateName, info.layer, info.normalizedTime);
                }
                break;
        }
    }
}
