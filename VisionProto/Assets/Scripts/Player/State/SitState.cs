using System.Collections;
using UnityEngine;

public class SitState : BaseState
{
    public SitState(PlayerStateMachine stateMachine) : base(stateMachine) { }
    float speed;
    bool isExiting;
    float transitionTime; // 전환에 사용되는 시간
    float startTime;
    Vector3 initialHeadPosition;
    private int grapplingLayer;
    private int grapplingPointLayer;

    public override void Enter()
    {
        grapplingLayer = LayerMask.GetMask("Grappling");
        grapplingPointLayer = LayerMask.GetMask("GrapplingPoint");

        stateMachine.capsuleCollider.center = new Vector3(0, -0.3f, 0);
        stateMachine.capsuleCollider.height = 1.2f;
        stateMachine.input.isSit = true;
        speed = stateMachine.moveSpeed;
        stateMachine.moveSpeed /= 2f;
        isExiting = false;

        stateMachine.sitPos = new Vector3(0, 0.125f, 0);

        // 초기 값 설정
        transitionTime = 0.5f;  // 전환에 사용할 시간 (초)
        startTime = Time.time;  // 전환 시작 시간 기록
        initialHeadPosition = stateMachine.head.localPosition; // 현재 머리 위치를 저장

        SoundManager.Instance.PlayEffectSound(SFX.Player_sit, stateMachine.transform);
    }

    public override void Tick()
    {
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            stateMachine.SwitchState(new IdleState(stateMachine));
        }
        if (Input.GetKeyDown(KeyCode.Space) && !stateMachine.input.isJump)
        {
            isExiting = true;
            stateMachine.input.isJump = false;
            stateMachine.SwitchState(new JumpState(stateMachine));
        }

        if (Input.GetMouseButtonDown(1))
        {
            isExiting = true;
            stateMachine.ObjectInteraction();

            if (stateMachine.layerMask == grapplingLayer || stateMachine.layerMask == grapplingPointLayer)
                stateMachine.SwitchState(new GrapplingState(stateMachine));
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            isExiting = true;
            stateMachine.SwitchState(new InteractionState(stateMachine));
        }
    }

    public override void FixedTick()
    {
        MoveDirection();
        Move();

        float t = (Time.time - startTime) / transitionTime;
        if (t < 1f)
        {
            stateMachine.head.localPosition = Vector3.Lerp(initialHeadPosition, stateMachine.sitPos, t);
        }
        else
        {
            stateMachine.head.localPosition = stateMachine.sitPos;
        }

        if (isExiting)
        {
            if (!stateMachine.input.isJump)
                stateMachine.SwitchState(new IdleState(stateMachine));
        }
    }

    public override void Exit()
    {
        stateMachine.moveSpeed = speed;
    }
}