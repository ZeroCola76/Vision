using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using Unity.VisualScripting;
using UnityEngine;

public class MoveState : BaseState
{
    public MoveState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    float speed;
    float lastVPStateTime;
    float coolTime = 5f;
    bool coolActive = false;

    GameObject walkSound;

    public override void Enter()
    {
        stateMachine.capsuleCollider.center = new Vector3(0, 0, 0);
        stateMachine.capsuleCollider.height = 2f;
        stateMachine.velocity.y = Physics.gravity.y;
        speed = stateMachine.moveSpeed;

        AudioSource audio = SoundManager.Instance.PlayAudioSourceEffectSound(SFX.Player_Walk, stateMachine.transform);
        walkSound = audio.gameObject;
    }

    public override void Tick()
    {
        stateMachine.animator.Speed = 0.5f;
        if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) &&
            !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D))
            stateMachine.SwitchState(new IdleState(stateMachine));
        if (Input.GetKeyDown(KeyCode.Space))
            stateMachine.SwitchState(new JumpState(stateMachine));
        if(!stateMachine.isVPState)
        {
            if (Input.GetKey(KeyCode.LeftShift))
                stateMachine.SwitchState(new RunState(stateMachine));
            if (Input.GetKeyDown(KeyCode.LeftControl))
                stateMachine.SwitchState(new SlideState(stateMachine));
        }
        if (stateMachine.isVPState)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
                stateMachine.SwitchState(new DashState(stateMachine));
        }
        if (!stateMachine.input.isGrounded)
            stateMachine.SwitchState(new FallState(stateMachine));
        if (Input.GetKeyDown(KeyCode.F))
            stateMachine.SwitchState(new InteractionState(stateMachine));

        if(Input.GetKeyDown(KeyCode.R))
        {
            if(!coolActive || Time.time >= lastVPStateTime + coolTime)
            {
                stateMachine.SwitchState(new VPState(stateMachine));
                lastVPStateTime = Time.time;
                coolActive = true;
            }
        }
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
        MoveDirection();
        Move();
    }
    public override void Exit()
    {
        SoundManager.Instance.DestroyObject(walkSound);
    }
}
