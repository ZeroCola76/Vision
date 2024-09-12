using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : BaseState
{
    public IdleState(PlayerStateMachine stateMachine) : base(stateMachine) { }
    Coroutine resetCoroutine;
    float lastVPStateTime;
    float coolTime = 5f;
    bool coolActive = false;


    public override void Enter()
    {
        stateMachine.capsuleCollider.center = new Vector3 (0, 0, 0);
        stateMachine.capsuleCollider.height = 2f;
        stateMachine.velocity = Vector3.zero;
        stateMachine.rigid.velocity = Vector3.zero;
        stateMachine.originPos = new Vector3(0, 0.725f, 0);
        stateMachine.head.localPosition = stateMachine.originPos;
        if (resetCoroutine != null)
        {
            stateMachine.StopCoroutine(resetCoroutine);
            resetCoroutine = null;
        }

        if (stateMachine.input.isSit && resetCoroutine == null)
        {
            resetCoroutine = stateMachine.StartCoroutine(LerpHeadPosition(stateMachine));
        }

    }

    public override void Tick()
    {
        stateMachine.animator.Speed = 0;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) ||
            Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
            stateMachine.SwitchState(new MoveState(stateMachine));
        if (Input.GetKeyDown(KeyCode.Space))
            stateMachine.SwitchState(new JumpState(stateMachine));
        if(!stateMachine.isVPState)
        {
            if (Input.GetKey(KeyCode.LeftControl))
                stateMachine.SwitchState(new SitState(stateMachine));
            if (Input.GetKey(KeyCode.LeftShift))
                stateMachine.SwitchState(new RunState(stateMachine));
        }
        if(stateMachine.isVPState)
        {
            if(Input.GetKeyDown(KeyCode.LeftShift))
                stateMachine.SwitchState(new DashState(stateMachine));
        }

        if (Input.GetMouseButtonDown(1) && !stateMachine.isVPState)
        {
            stateMachine.ObjectInteraction();

            //if (stateMachine.layerMask == grapplingLayer || stateMachine.layerMask == grapplingPointLayer)
            if (stateMachine.targetGameObject.CompareTag("GrapplingPoint") || stateMachine.targetGameObject.CompareTag("Grappling"))
                stateMachine.SwitchState(new GrapplingState(stateMachine));
        }
        if (Input.GetKeyDown(KeyCode.F))
            stateMachine.SwitchState(new InteractionState(stateMachine));
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!coolActive || Time.time >= lastVPStateTime + coolTime)
            {
                stateMachine.SwitchState(new VPState(stateMachine));
                lastVPStateTime = Time.time;
                coolActive = true;
            }
        }
    }

    public override void FixedTick()
    {

    }

    public override void Exit()
    {
        if (resetCoroutine != null)
        {
            stateMachine.StopCoroutine(resetCoroutine);
            resetCoroutine = null;
        }
    }

    private IEnumerator LerpHeadPosition(PlayerStateMachine stateMachine)
    {
        Vector3 startPosition = stateMachine.sitPos;
        Vector3 targetPosition = stateMachine.originPos;

        float duration = 0.5f;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            stateMachine.head.localPosition = Vector3.Lerp(startPosition, targetPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        stateMachine.head.localPosition = targetPosition;
        stateMachine.input.isSit = false;
        resetCoroutine = null;
    }
}
