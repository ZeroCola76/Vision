using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunState : BaseState
{
    public RunState(PlayerStateMachine stateMachine) : base(stateMachine) { }
    float speed;

    // Grappling Point Layer와 Grappling Layer를 여기다가 둘까 아니면 각자 필요한 곳에 둘까
    private int grapplingLayer;
    private int grapplingPointLayer;

    GameObject runSound;

    public override void Enter()
    {
        grapplingLayer = LayerMask.GetMask("Grappling");
        grapplingPointLayer = LayerMask.GetMask("GrapplingPoint");

        stateMachine.velocity.y = Physics.gravity.y;
        speed = stateMachine.moveSpeed;
        stateMachine.moveSpeed += 5f;

        AudioSource audio = SoundManager.Instance.PlayAudioSourceEffectSound(SFX.Player_Run, stateMachine.transform);
        runSound = audio.gameObject;

    }
    public override void Tick()
    {
        stateMachine.animator.Speed = 1;

        if(!stateMachine.isVPState)
        {
            if (Input.GetKeyDown(KeyCode.Space))
                stateMachine.SwitchState(new JumpState(stateMachine));
            if (Input.GetKeyUp(KeyCode.LeftShift))
                stateMachine.SwitchState(new MoveState(stateMachine));
            if (Input.GetKeyDown(KeyCode.LeftControl))
                stateMachine.SwitchState(new SlideState(stateMachine));
        }
        if(!stateMachine.input.isGrounded)
            stateMachine.SwitchState(new FallState(stateMachine));
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
        stateMachine.moveSpeed = speed;
        SoundManager.Instance.DestroyObject(runSound);
    }
}
