using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashState : BaseState
{
    Vector3 dashDirection;
    Vector3 dashStartPos;
    float dashStartTime;
    bool isExiting;

    public DashState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        dashDirection = stateMachine.direction.forward.normalized;
        dashStartPos = stateMachine.transform.position;
        dashStartTime = Time.time;
        isExiting = false;
        stateMachine.input.isSit = false;
        stateMachine.capsuleCollider.center = new Vector3(0, 0f, 0);
        stateMachine.capsuleCollider.height = 2f;
        stateMachine.head.localPosition = stateMachine.originPos;
        stateMachine.input.dashIndex++;
        stateMachine.rigid.useGravity = false;
        stateMachine.input.dashDamage = true;
    }

    public override void Tick()
    {
        if (!isExiting && Input.GetKeyUp(KeyCode.LeftShift))
        {
            isExiting = true;
        }
    }

    public override void FixedTick()
    {
        if (stateMachine.input.isGrounded || stateMachine.input.dashIndex == 1)
        {
            float t = (Time.time - dashStartTime) / stateMachine.dashDuration;
            if (t <= 1f)
            {
                float smoothStep = t * t * (3f - 2f * t);
                Vector3 dashVelocity = dashDirection * stateMachine.dashSpeed * smoothStep;

                if (CheckSlope())
                {
                    dashVelocity = AdjustDirectionToSlope(dashVelocity);
                }

                if (stateMachine.input.isGrounded)
                {
                    stateMachine.velocity = dashVelocity;
                }
                else
                {
                    //점프하면서 대각선으로 내려가는 아이
                    stateMachine.velocity = new Vector3(dashVelocity.x, stateMachine.velocity.y, dashVelocity.z);
                }

                stateMachine.rigid.velocity = stateMachine.velocity*stateMachine.VPSpeed;
            }
        }

        Gravity();
        /*Move();*/

        if (!stateMachine.input.isGrounded && stateMachine.input.dashIndex > 1)
        {
            isExiting = true;
        }

        if (Vector3.Distance(dashStartPos, stateMachine.transform.position) > stateMachine.dashDistance)
        {
            isExiting = true;
        }

        if (isExiting)
        {
            stateMachine.SwitchState(new MoveState(stateMachine));
        }
    }

    public override void Exit()
    {
        stateMachine.velocity = Vector3.zero;
        stateMachine.rigid.useGravity = true;
        stateMachine.input.dashDamage = false;
    }
    protected void Gravity()
    {
        // 평지나 공중에서는 수직으로 중력을 적용
        if (stateMachine.velocity.y > Physics.gravity.y)
        {
            stateMachine.velocity += Vector3.up * Physics.gravity.y * Time.deltaTime;
        }
    }
}
