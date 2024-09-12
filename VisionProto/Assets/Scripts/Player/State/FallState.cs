using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class FallState : BaseState
{
    public FallState(PlayerStateMachine stateMachine) : base(stateMachine) { }
    float maxFallTime = 1.5f; // 최대 낙하 시간
    float startTime;

    public override void Enter()
    {
        stateMachine.input.dashIndex = 1;
        stateMachine.velocity.y = 0f;
        startTime = Time.time;
    }
    public override void Tick()
    {
        if (Input.GetKeyDown(KeyCode.F))
            stateMachine.SwitchState(new InteractionState(stateMachine));
        if (stateMachine.isVPState)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
                stateMachine.SwitchState(new DashState(stateMachine));
            if(Input.GetKeyDown(KeyCode.LeftControl))
                stateMachine.SwitchState(new SlashState(stateMachine));
        }
    }

    public override void FixedTick()
    {
        ApplyGravity();
        Move();
        if (stateMachine.input.isGrounded)
        {
            stateMachine.SwitchState(new MoveState(stateMachine));
            stateMachine.input.isJump = false;
        }
        if (Time.time - startTime > maxFallTime)
        {
            stateMachine.SwitchState(new MoveState(stateMachine));
        }
    } 
    public override void Exit()
    {
        
    }
}
