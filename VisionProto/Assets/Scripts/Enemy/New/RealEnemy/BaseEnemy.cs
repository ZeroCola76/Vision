using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static BTNode;
using static ChaseAction;

public abstract class BaseEnemy : MonoBehaviour, IListener
{
    protected BTNode baseBehaviorTree;
    protected Dictionary<string, EnemyAction> actions;

    public Transform player;
    public Transform npceye;
    public Transform[] upperBody;
    public Transform[] child;

    private Vector3 detectionCenter; 

    public float detectionRadius; 

    public float detectionAngle;

    public float detectionSound;

    private BoolWrapper something = new BoolWrapper(false);
    private BoolWrapper npchitted = new BoolWrapper(false);
    public NavMeshAgent agent;
    protected Animator animator;

    public FloatWrapper HP = new FloatWrapper(50f);
    public int enemyType = 0;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        if (player == null)
        {
            player = GameObject.Find("Head").transform;
        }
        
        detectionCenter = npceye.transform.position;

        InitializeActions();
        InitializeBehaviorTree();
        EventManager.Instance.AddEvent(EventType.playerShot, OnEvent);
        //EventManager.Instance.AddEvent(EventType.NPCHit, OnEvent);
    }

    // Update is called once per frame
    protected void Update()
    {
        if (HP.Value <= 0)
        {
            actions["PatrolAction"].Stop();
            actions["ChaseAction"].Stop();
            actions["DieAction"].Execute();
        }
        else if (npchitted.Value)
        {
            actions["PatrolAction"].Stop();
            actions["ChaseAction"].Stop();
            actions["HurtAction"].Execute();
            StartCoroutine(ResetNpcHitted());
        }
        else
        {
            NodeState result = baseBehaviorTree.Execute();
            Debug.Log($"{GetType().Name} Behavior Tree Result: " + result);
        }
        //NodeState result = baseBehaviorTree.Execute();
        //Debug.Log($"{GetType().Name} Behavior Tree Result: " + result);
    }

    protected abstract void InitializeBehaviorTree();

    protected virtual void InitializeActions()
    {
        actions = new Dictionary<string, EnemyAction>
        {
            { "IdleAction", new IdleSightAction(agent,player, npceye, detectionRadius, detectionAngle,animator) },
            { "IdleSAction", new IdleSoundAction(player, npceye, detectionRadius,something ) },
            { "PatrolAction", new PatrolAction(player, npceye, agent,detectionRadius,child[0].transform,enemyType,animator) },
            { "ChaseAction", new ChaseAction(player, agent, child[0].transform,animator) },
            { "HurtAction", new HurtAction(agent,HP,npchitted, animator) },
            { "DieAction", new DieAction(agent,HP, animator) }
        };
    }

    public void OnEvent(EventType eventType, object param = null)
    {
        switch (eventType)
        {
            case EventType.playerShot:
                {
                    if (param is bool shot)
                    {
                        something.Value = shot;
                    }
                }
                break;
            //case EventType.NPCHit:
            //    {
            //        npchitted = (bool)param;
            //    }
            //    break;
            //case EventType.playerShot:
            //    {
            //        actions["HurtAction"].Execute();
            //    }
            //    break;
            default:
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet") || other.CompareTag("ThrowWeapon"))
        {
            npchitted.Value = true;
        }
    }

    private IEnumerator ResetNpcHitted()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            npchitted.Value = false;
            yield return null;
        }
    }
}

public class BoolWrapper
{
    public bool Value;

    public BoolWrapper(bool value)
    {
        Value = value;
    }
}

public class FloatWrapper
{
    public float Value;

    public FloatWrapper(float value)
    {
        Value = value;
    }
}