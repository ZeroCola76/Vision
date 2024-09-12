using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 상호작용하기 전에 Layer를 체크하는 State다.
/// </summary>
public class ObjectOpenState : BaseState
{
    public ObjectOpenState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    OpenWeaponCabinet weaponCabinet;
    OpenItem openItem;
    // OpenItem

    public override void Enter()
    {
        if( stateMachine.objectTag == "Cabinet")
        {
            weaponCabinet = stateMachine.targetGameObject.GetComponent<OpenWeaponCabinet>();
            if(weaponCabinet != null ) 
            {
                weaponCabinet.isControl = true;
            }
        }
        else if( stateMachine.objectTag == "Item")
        {
            // item open
            openItem = stateMachine.targetGameObject.GetComponent<OpenItem>();
            if( openItem != null ) 
            {
                openItem.isControl = true;
            }
        }
    }

    public override void Tick()
    {
        stateMachine.SwitchState(new IdleState(stateMachine));
    }

    public override void FixedTick()
    {

    }

    public override void Exit()
    {
        stateMachine.targetGameObject = null;
        stateMachine.objectTag = default;
    }
}
