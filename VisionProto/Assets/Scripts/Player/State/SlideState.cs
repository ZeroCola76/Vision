using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SlideState : BaseState
{
    Vector3 SlideDirection;
    Vector3 SlideStartPos;
    float slideStartTime;
    bool isExiting;

    public SlideState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        stateMachine.capsuleCollider.center = new Vector3(0, -0.3f, 0);
        stateMachine.capsuleCollider.height = 1.2f;
        SlideDirection = stateMachine.direction.forward.normalized;
        SlideStartPos = stateMachine.transform.position;
        slideStartTime = Time.time;
        isExiting = false;
        SoundManager.Instance.PlayEffectSound(SFX.Player_Slide, stateMachine.transform);
    }

    public override void Tick()
    {

    }

    public override void FixedTick()
    {
        float t = (Time.time - slideStartTime) / stateMachine.slideDuration;

        if (t <= 1f)
        {
            float smoothStep = t * t * (3f - 2f * t);
            stateMachine.velocity = SlideDirection * stateMachine.slideSpeed * smoothStep;
        }

        Move();

        if (Time.time - slideStartTime >= stateMachine.slideDuration || Vector3.Distance(SlideStartPos, stateMachine.transform.position) > stateMachine.slideDistance)
        {
            isExiting = true;
        }

        if (isExiting)
        {
            stateMachine.head.localPosition = Vector3.Lerp(stateMachine.head.localPosition, stateMachine.originPos, 0.5f);
            if (Vector3.Distance(stateMachine.head.localPosition, stateMachine.originPos) < 0.01f)
            {
                stateMachine.SwitchState(new MoveState(stateMachine));
            }
            stateMachine.SwitchState(new MoveState(stateMachine));
        }
        else
        {
            stateMachine.head.localPosition = Vector3.Lerp(stateMachine.head.localPosition, stateMachine.sitPos, t);
        }
    }

    public override void Exit()
    {
        stateMachine.velocity = Vector3.zero;
    }
}
