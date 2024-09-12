using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashState : BaseState
{
    public SlashState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        stateMachine.input.slashDamage = true;
        stateMachine.boxCollider.enabled = true;
    }

    public override void Tick()
    {
        
    }

    public override void FixedTick()
    {
        ApplyGravity();
        Move();
        Vector3 velocity = stateMachine.velocity;
        if(velocity.y < 0f)
            velocity.y *= stateMachine.fallSpeed;
        stateMachine.velocity = velocity;

        //if (stateMachine.input.isGrounded)
        if (stateMachine.input.isGrounded)
            stateMachine.SwitchState(new MoveState(stateMachine));
    }
    public override void Exit()
    {
        stateMachine.slashPoint = stateMachine.transform.position;
        stateMachine.input.slashDamage = false;
        stateMachine.boxCollider.enabled = false;
    }
}
