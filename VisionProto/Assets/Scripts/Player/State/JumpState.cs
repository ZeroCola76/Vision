using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class JumpState : BaseState
{
    public JumpState(PlayerStateMachine stateMachine) : base(stateMachine) { }


    float lastVPStateTime;
    float coolTime = 5f;
    bool coolActive = false;

    public override void Enter()
    {
        if (stateMachine.input.isSit)
        {
            stateMachine.input.isJump = true;
            stateMachine.head.localPosition = stateMachine.sitPos;
        }
        if(stateMachine.isVPState)
            stateMachine.velocity = new Vector3(stateMachine.velocity.x, stateMachine.VPJumeForce, stateMachine.velocity.z);
        else
            stateMachine.velocity = new Vector3(stateMachine.velocity.x, stateMachine.jumpForce, stateMachine.velocity.z);

        //stateMachine.rigid.velocity = new Vector3(stateMachine.rigid.velocity.x, 0f, stateMachine.rigid.velocity.z);
        //stateMachine.rigid.AddForce(Vector3.up * stateMachine.jumpForce*100f, ForceMode.Impulse);
        //stateMachine.rigid.velocity = new Vector3(stateMachine.rigid.velocity.x, stateMachine.jumpForce*100f, stateMachine.rigid.velocity.z);
        stateMachine.input.dashIndex = 0;

        SoundManager.Instance.PlayEffectSound(SFX.Player_Jump, stateMachine.transform);
    }
    public override void Tick()
    {
        if(stateMachine.isVPState)
        {
//             if (Input.GetKeyDown(KeyCode.LeftControl))
//                 stateMachine.SwitchState(new SlashState(stateMachine));
            if (Input.GetKeyDown(KeyCode.LeftShift))
                stateMachine.SwitchState(new DashState(stateMachine));
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!coolActive || Time.time >= lastVPStateTime + coolTime)
            {
                stateMachine.SwitchState(new VPState(stateMachine));
                lastVPStateTime = Time.time;
                coolActive = true;
            }
        }
        // tick에서 계속 검사하는건 좀 그래
        if (Input.GetMouseButtonDown(1) && !stateMachine.isVPState)
        {
            stateMachine.ObjectInteraction();

            //if (stateMachine.layerMask == grapplingLayer || stateMachine.layerMask == grapplingPointLayer)
            if (stateMachine.targetGameObject.CompareTag("GrapplingPoint") || stateMachine.targetGameObject.CompareTag("Grappling"))
                stateMachine.SwitchState(new GrapplingState(stateMachine));
        }
    }
    public override void FixedTick()
    {
        ApplyGravity();
        Move();

        if (stateMachine.velocity.y <= 0f)
        {
            stateMachine.SwitchState(new FallState(stateMachine));
        }

    }
    public override void Exit()
    {
        if (!stateMachine.input.isSit)
        {
            stateMachine.input.isSit = false;
            stateMachine.head.localPosition = stateMachine.originPos;
        }
        stateMachine.layerMask = 0;
    }
}