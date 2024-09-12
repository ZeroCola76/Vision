using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class VPState : BaseState
{
    public VPState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        if (stateMachine.isVPState)
        {
            stateMachine.isVPState = false;

            if (stateMachine.VPStateRange.activeSelf)
                stateMachine.VPStateRange.SetActive(false);
        }
        else
        {
            stateMachine.isVPState = true;
            
            if(!stateMachine.VPStateRange.activeSelf)
                stateMachine.VPStateRange.SetActive(true);
        }
        EventManager.Instance.NotifyEvent(EventType.VPState, stateMachine.isVPState);
        
    }

    public override void Tick()
    {
        stateMachine.SwitchPreviousState();
    }

    public override void FixedTick()
    {

    }

    public override void Exit()
    {

    }

}