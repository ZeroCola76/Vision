using System.Collections;
using System.Collections.Generic;
using System.Security.Authentication.ExtendedProtection;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

[DefaultExecutionOrder(-1)]
public abstract class BaseState : IState
{
    protected readonly PlayerStateMachine stateMachine;
    protected RaycastHit slopeHit;
    private int groundLayer = 1 << LayerMask.NameToLayer("Floor");

    protected BaseState(PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    protected void MoveDirection()
    {
        stateMachine.velocity = stateMachine.direction.forward.normalized * stateMachine.input.vAxis
            + stateMachine.direction.right.normalized * stateMachine.input.hAxis;
    }

    protected void ApplyGravity()
    {
        if (stateMachine.velocity.y > Physics.gravity.y)
        {
            stateMachine.velocity.y += Physics.gravity.y * Time.deltaTime;
        }
    }
    protected void Move()
    {
        bool isOnSlope = CheckSlope();
        bool isGrounded = IsGrounded();
        Vector3 velocity = stateMachine.velocity;
        //Vector3 gravity = Vector3.down * Mathf.Abs(stateMachine.rigid.velocity.y);

        if(stateMachine.input.isGrounded && isOnSlope)
        {
            velocity = AdjustDirectionToSlope(velocity);
            //gravity = Vector3.zero;
            stateMachine.rigid.useGravity = false;
        }
        else
            stateMachine.rigid.useGravity = true;
        if (stateMachine.isVPState)
            stateMachine.rigid.velocity = velocity * stateMachine.VPSpeed;
        else
            stateMachine.rigid.velocity = velocity * stateMachine.moveSpeed;
        /* + gravity*/;
        //stateMachine.rigid.velocity = stateMachine.velocity * stateMachine.moveSpeed;
    }

    protected Vector3 AdjustDirectionToSlope(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }

    public bool CheckSlope()
    {
        Ray ray = new Ray(stateMachine.transform.position, Vector3.down);
        if (Physics.Raycast(ray, out slopeHit, 11f, groundLayer))
        {
            var angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            //Debug.Log($"Slope Angle: {angle}, Slope Normal: {slopeHit.normal}");
            return angle != 0f && angle < 70f;
        }
        return false;
    }

    public bool IsGrounded()
    {
        Vector3 boxSize = new Vector3(stateMachine.transform.lossyScale.x, 1f, stateMachine.transform.lossyScale.z);
        return Physics.CheckBox(stateMachine.groundCheck.position, boxSize, Quaternion.identity, groundLayer);
    }
}