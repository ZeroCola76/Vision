using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class PistolEnemy : BaseEnemy
{
    protected override void Start()
    {
        detectionRadius = 150f; 
        detectionAngle = 160f;
        detectionSound = 100f;
        //HP.Value = 50;    
        agent = this.GetComponent<NavMeshAgent>();
        animator = this.GetComponent<Animator>();
        child = gameObject.transform.GetComponentsInChildren<Transform>();
        Transform[] reyeChildren = child.Where(child => child.name == "Reye").ToArray();
        child = reyeChildren;
        upperBody = gameObject.transform.GetComponentsInChildren<Transform>();
        Transform[] upperChildren = upperBody.Where(child => child.name == "DEF-spine.001").ToArray();
        upperBody = upperChildren;
        enemyType = 0;

        base.Start();
    }

    protected override void InitializeBehaviorTree()
    {
        var idlesequenceNode = new SequenceNode();
        idlesequenceNode.AddChild(new ActionNode(() => actions["IdleAction"].Execute()));
        idlesequenceNode.AddChild(new ActionNode(() => actions["IdleSAction"].Execute()));

        var patrolSequence = new SequenceNode();
        patrolSequence.AddChild(new ActionNode(() => actions["PatrolAction"].Execute()));

        var chaseSequence = new SequenceNode();
        chaseSequence.AddChild(new ActionNode(() => actions["ChaseAction"].Execute()));

        var hurtSequence = new SequenceNode();
        hurtSequence.AddChild(new ActionNode(() => actions["HurtAction"].Execute()));

        var dieSequence = new SequenceNode();
        dieSequence.AddChild(new ActionNode(() => actions["DieAction"].Execute()));

        baseBehaviorTree = new SelectorNode();
        ((SelectorNode)baseBehaviorTree).AddChild(idlesequenceNode);
        ((SelectorNode)baseBehaviorTree).AddChild(patrolSequence);
        ((SelectorNode)baseBehaviorTree).AddChild(chaseSequence);
        ((SelectorNode)baseBehaviorTree).AddChild(hurtSequence);
        ((SelectorNode)baseBehaviorTree).AddChild(dieSequence);
    }

}
