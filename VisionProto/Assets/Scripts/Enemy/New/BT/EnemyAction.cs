using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR;
using static BTNode;

public abstract class EnemyAction
{
    public abstract NodeState Execute();
    public abstract NodeState Stop();

    protected void StartCoroutine(IEnumerator coroutine)
    {
        CoroutineRunner.Instance.StartCoroutine(coroutine);
    }
}

public class IdleSightAction : EnemyAction
{
    private Transform player;
    private Transform center;
    private float radius;
    private float detectionAngle;
    private Animator animator;
    private NavMeshAgent agent;

    public IdleSightAction(NavMeshAgent agent, Transform player, Transform center, float radius, float detectionAngle, Animator animator)
    {
        this.player = player;
        this.center = center;
        this.radius = radius;
        this.detectionAngle = detectionAngle;
        this.animator = animator;
        this.agent = agent;
    }

    public override NodeState Execute()
    {
        agent.isStopped = false;
        float distanceToCenter = Vector3.Distance(player.position, center.transform.position);
        animator.Play("Idle_1");

        if (distanceToCenter <= radius)
        {

            Vector3 toPlayer = player.position - center.transform.position;
            Vector3 toPlayerFlat = toPlayer;
            toPlayerFlat.y = 0;

            float angleToPlayer = Vector3.Angle(toPlayerFlat, agent.transform.forward);

            float halfDetectionAngle = detectionAngle / 2f;

            if (angleToPlayer <= halfDetectionAngle)
            {
                return NodeState.Failure;
            }
        }

        Debug.Log("Idle ������...");
        return NodeState.Success;
    }

    public override NodeState Stop()
    {
        ///��� �������� ��ž�� ���� �ʿ䰡 �ֳ�?
        return NodeState.Failure;
    }
}

public class IdleSoundAction : EnemyAction
{
    private Transform player;
    private Transform center;
    private float radius;
    private BoolWrapper isShot;

    public IdleSoundAction(Transform player, Transform center, float radius, BoolWrapper isShot)
    {
        this.player = player;
        this.center = center;
        this.radius = radius;
        this.isShot = isShot;
    }

    public override NodeState Execute()
    {
        float distanceToCenter = Vector3.Distance(player.position, center.transform.position);

        if (distanceToCenter <= radius)
        {
            if (isShot.Value)
            {
                return NodeState.Failure;
            }
        }

        Debug.Log("sIdle ������...");
        return NodeState.Success;
    }

    public override NodeState Stop()
    {
        ///��� �������� ��ž�� ���� �ʿ䰡 �ֳ�?
        return NodeState.Failure;
    }
}

public class PatrolAction : EnemyAction
{
    private Transform player;
    private Transform center;
    private NavMeshAgent navmeshEnemy;
    private float radius;
    private Vector3 currentDestination;
    private bool isMoving = false;
    private Animator animator;
    private Transform eye;
    private Coroutine spawnCoroutine;
    private float timer;
    private int npcType;

    public PatrolAction(Transform player, Transform center, NavMeshAgent navMeshAgent, float radius , Transform eye,int npcType,Animator animator )
    {
        this.player = player;
        this.center = navMeshAgent.transform;
        this.navmeshEnemy = navMeshAgent;
        this.radius = radius;
        this.animator = animator;
        this.eye = eye;
        navmeshEnemy.isStopped = false;
        this.npcType = npcType;
    }

    public override NodeState Execute()
    {
        navmeshEnemy.isStopped = false; //��� ���⿡ �ִ°� ��� �ȵȴ�..
        navmeshEnemy.updateRotation = false;
        Debug.Log("Patrol������" + isMoving);

        Vector3 directionToPlayer = (player.position - navmeshEnemy.transform.position).normalized;

        if (directionToPlayer != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);

            
            Vector3 rotation = Quaternion.Slerp(navmeshEnemy.transform.rotation, lookRotation, Time.deltaTime * navmeshEnemy.angularSpeed).eulerAngles;
            navmeshEnemy.transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);

        }

        bool isMovingBackward = IsMovingBackward(navmeshEnemy, directionToPlayer);
        if (isMovingBackward)
        {
            //Debug.Log("Agent�� �ڷ� �̵� ��");
            animator.Play("WalkBackwards");
        }
        else if (navmeshEnemy.isOnOffMeshLink)
        {
            animator.Play("Jump");
        }
        else
        {
            animator.Play("chase");
        }

        float distanceToPlayer = Vector3.Distance(player.position, center.transform.position);
        timer += Time.deltaTime;
        if (timer >= 1 && spawnCoroutine == null)
        {
            if (npcType == 0)
            {
                spawnCoroutine = CoroutineRunner.Instance.StartCoroutine(SpawnBullet());
            }
            else if (npcType == 1)
            {
                spawnCoroutine = CoroutineRunner.Instance.StartCoroutine(SpawnBullet2());
            }
            else if (npcType == 2)
            {
                spawnCoroutine = CoroutineRunner.Instance.StartCoroutine(SpawnBullet3());
            }
            timer = 0f;
        }

        if (distanceToPlayer >= radius || Input.GetKeyDown(KeyCode.K))
        {
            //Debug.Log("�Ѿ���?");
            if (spawnCoroutine != null)
            {
                CoroutineRunner.Instance.StopCoroutine(spawnCoroutine);
                spawnCoroutine = null;
            }
            isMoving = false;
            return NodeState.Failure;
        }

        if (navmeshEnemy != null && player != null)
        {
           
            if (!isMoving || navmeshEnemy.remainingDistance <= navmeshEnemy.stoppingDistance)
            {
                isMoving = true; 

                
                Vector3 randomPosition = GetRandomPositionInCircle(player.transform.position, distanceToPlayer);
                navmeshEnemy.SetDestination(randomPosition);

                //Debug.Log("�̵��� ��ġ" + randomPosition);
            }

           
            if (navmeshEnemy.remainingDistance <= navmeshEnemy.stoppingDistance)
            {
                isMoving = false; 
                //return NodeState.Success; 
            }

            return NodeState.Running; 
        }

      

        return NodeState.Running;
    }

    public override NodeState Stop()
    {
        ///��� �������� ��ž�� ���� �ʿ䰡 �ֳ�?
        if (spawnCoroutine != null)
        {
            CoroutineRunner.Instance.StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
        isMoving = false;
        return NodeState.Failure;
    }

    private Vector3 GetRandomPositionInCircle(Vector3 center, float radius)
    {
        Vector2 randomDirection2D = Random.insideUnitCircle * radius;
        Vector3 randomDirection = new Vector3(randomDirection2D.x, 0, randomDirection2D.y); 

        Vector3 targetPosition = center + randomDirection;

        NavMeshHit navHit;
        if (NavMesh.SamplePosition(targetPosition, out navHit, radius, NavMesh.AllAreas))
        {
            Vector3 resultPosition = navHit.position;
            float[] possibleValues = { 1.2f, 32.3417f, 80.2f }; //�̺κ��� ���ذ� �ȵǴ°� ���� ����. ��� �� �Ǵ°� �̻��� ����. 
            resultPosition.y = possibleValues[Random.Range(0, possibleValues.Length)];

            if (!IsNearWall(resultPosition))
            {
                return resultPosition;
            }
        }

        return center; 
    }

    private bool IsNearWall(Vector3 position)
    {
        NavMeshHit hit;
        if (NavMesh.Raycast(position, position + Vector3.forward * 1.0f, out hit, NavMesh.AllAreas) ||
            NavMesh.Raycast(position, position - Vector3.forward * 1.0f, out hit, NavMesh.AllAreas) ||
            NavMesh.Raycast(position, position + Vector3.right * 1.0f, out hit, NavMesh.AllAreas) ||
            NavMesh.Raycast(position, position - Vector3.right * 1.0f, out hit, NavMesh.AllAreas))
        {
            return true; 
        }
        return false;
    }

    private IEnumerator SpawnBullet()
    {
        int bulletsSpawned = 0;
        const int maxBulletsToSpawn = 12;

        while (bulletsSpawned < maxBulletsToSpawn)
        {
            yield return new WaitForSeconds(0.3f);

            GameObject bullet = PoolManager.Instance.GetPooledObject("Ebullet");
            Bullet bullet1 = bullet.GetComponent<Bullet>();
            if (bullet != null)
            {
                bullet1.SetUp(eye.transform, player.transform);
                //Debug.Log(".�����߾ȳ�" + player.transform.position);
                bulletsSpawned++;

                ///TO DO : ����ü ���� �迹����
                //StartCoroutine(test(bullet, "Ebullet"));
            }
        }
        yield return new WaitForSeconds(3f);
        spawnCoroutine = null;
    }

    private IEnumerator SpawnBullet2()
    {
        int bulletsSpawned = 0;
        const int maxBulletsToSpawn = 30;

        while (bulletsSpawned < maxBulletsToSpawn)
        {
            yield return new WaitForSeconds(0.1f);

            GameObject bullet = PoolManager.Instance.GetPooledObject("Ebullet1");
            Bullet bullet2 = bullet.GetComponent<Bullet>();
            if (bullet != null)
            {
                bullet2.SetUp(eye.transform, player.transform);
                bulletsSpawned++;
            }
        }
        yield return new WaitForSeconds(4f);
        spawnCoroutine = null;
    }

    private IEnumerator SpawnBullet3()
    {
        int bulletsSpawned = 0;
        const int maxBulletsToSpawn = 6; // �� ������ �Ѿ� ��
        const int bulletsPerWave = 3; // �� ���� ������ �Ѿ� ��

        while (bulletsSpawned < maxBulletsToSpawn)
        {
            for (int i = 0; i < bulletsPerWave; i++)
            {
                GameObject bullet = PoolManager.Instance.GetPooledObject("Ebullet2");
                Bullet bullet4 = bullet.GetComponent<Bullet>();
                if (bullet != null)
                {
                    bullet4.SetUp(eye.transform, player.transform);
                    bulletsSpawned++;

                    if (bulletsSpawned >= maxBulletsToSpawn)
                    {
                        break;
                    }
                }
            }

            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(3f);
        spawnCoroutine = null;
    }

    //�� ���� �� �� �ִ°�? 
    public bool CanReach(Vector3 targetPosition)
    {
        NavMeshPath path = new NavMeshPath();
        navmeshEnemy.CalculatePath(targetPosition, path);

        return path.status == NavMeshPathStatus.PathComplete;
    }

    //�ڷ� ���� �ִ°ž�? 
    private bool IsMovingBackward(NavMeshAgent agent, Vector3 directionToPlayer)
    {
        // �̵� ���� (Velocity ����)�� �÷��̾ ���� ������ ��
        Vector3 agentVelocity = agent.velocity.normalized;
        float dotProduct = Vector3.Dot(agentVelocity, directionToPlayer);

        // dotProduct�� -0.5 ���϶��, �̵� ������ �÷��̾� �ݴ������� ���ϰ� �ִ� ����
        return dotProduct < -0.5f;
    }
}

public class ChaseAction : EnemyAction
{
    private Vector3 initialPlayerPosition;
    private NavMeshAgent navMeshAgent;
    private Transform player;
    private float chaseDuration = 3f; 
    private float chaseTimer = 0f;
    private bool hasReachedDestination = false;
    private bool isInitialized = false;
    private Animator animator;
    private float timer;
    private Coroutine spawnCoroutine;
    private Transform eye;

    public ChaseAction(Transform player, NavMeshAgent navMeshAgent, Transform eye, Animator animator)
    {
        this.player = player;
        this.navMeshAgent = navMeshAgent;
        this.animator = animator;
        this.eye = eye;


    }

    public override NodeState Execute()
    {
        navMeshAgent.isStopped = false;
        navMeshAgent.updateRotation = true;
        if (!isInitialized)
        {
            initialPlayerPosition = player.position;
            isInitialized = true;
            Debug.Log("�ʱ���ġ " + initialPlayerPosition);
        }

        timer += Time.deltaTime;
        if (timer >= 1 && spawnCoroutine == null)
        {
            spawnCoroutine = CoroutineRunner.Instance.StartCoroutine(SpawnBullet());
            timer = 0f;
        }

        Debug.Log("chase ������...");
        animator.Play("chase");


        if (!hasReachedDestination)
        {
            navMeshAgent.SetDestination(initialPlayerPosition);


            if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f)
                {
                    hasReachedDestination = true;
                    isInitialized = false;
                    chaseTimer = 0f; 
                    Debug.Log("��ǥ ��ġ ����");
                    if (spawnCoroutine != null)
                    {
                        CoroutineRunner.Instance.StopCoroutine(spawnCoroutine);
                        spawnCoroutine = null;
                    }
                    return NodeState.Failure;
                }
            }
        }
        else
        {
            
            chaseTimer += Time.deltaTime;

            
            if (chaseTimer >= chaseDuration)
            {
                isInitialized = false;
                if (spawnCoroutine != null)
                {
                    CoroutineRunner.Instance.StopCoroutine(spawnCoroutine);
                    spawnCoroutine = null;
                }
                return NodeState.Failure;
            }
        }

        return NodeState.Running;
    }

    public override NodeState Stop()
    {
        ///��� �������� ��ž�� ���� �ʿ䰡 �ֳ�?
        if (spawnCoroutine != null)
        {
            CoroutineRunner.Instance.StopCoroutine(spawnCoroutine);
        }
        return NodeState.Failure;
    }

    private IEnumerator SpawnBullet()
    {
        int bulletsSpawned = 0;
        const int maxBulletsToSpawn = 12;

        while (bulletsSpawned < maxBulletsToSpawn)
        {
            yield return new WaitForSeconds(0.3f);

            GameObject bullet = PoolManager.Instance.GetPooledObject("Ebullet");
            Bullet bullet1 = bullet.GetComponent<Bullet>();
            if (bullet != null)
            {
                bullet1.SetUp(eye.transform, player.transform);
                Debug.Log(".�����߾ȳ�" + player.transform);
                bulletsSpawned++;

                ///TO DO : ����ü ���� �迹����
                //StartCoroutine(test(bullet, "Ebullet"));
            }
        }
        yield return new WaitForSeconds(3f);
        spawnCoroutine = null;
    }
}
public class HurtAction : EnemyAction
{
    private FloatWrapper hpWrapper;
    private Animator animator;
    private NavMeshAgent agent;
    private BoolWrapper isHit = new BoolWrapper(false);
    private bool isWaiting = false;


    public HurtAction(NavMeshAgent agent ,FloatWrapper hpWrapper, BoolWrapper ishit, Animator animator)
    {
        this.hpWrapper = hpWrapper;
        this.animator = animator;
        this.agent = agent;
        isHit = ishit;
    }

    public override NodeState Execute()
    {
        Debug.Log("�ǰ� ������");

        //if(!isHit.Value)
        //{
        //    return NodeState.Failure;
        //}

        if (hpWrapper != null && !isWaiting)
        {
            agent.isStopped = true;
            animator.Play("Hurt");

            // �ڷ�ƾ ����
            StartCoroutine(WaitAndReturn());
            isWaiting = true; // �ߺ� ȣ�� ����
            return NodeState.Running; // �ڷ�ƾ�� ���� ���� ���� Running ����
        }

        return NodeState.Failure;
    }

    public override NodeState Stop()
    {
        ///��� �������� ��ž�� ���� �ʿ䰡 �ֳ�?
        return NodeState.Failure;
    }

    private IEnumerator WaitAndReturn()
    {
        // 2�� ���
        yield return new WaitForSeconds(0.5f);

        // 2�� �ڿ� ������ �ڵ�
        isWaiting = false;
        yield break; // �ڷ�ƾ ����
    }
}

public class DieAction : EnemyAction
{
    private NavMeshAgent agent;
    Transform a;
    private Rigidbody rigid;
    Animator animator;
    private FloatWrapper isHit;
    public DieAction(NavMeshAgent agent, FloatWrapper isHit, Animator animator)
    {
        this.agent = agent;
        a = agent.transform;
        this.animator = animator;
        this.isHit = isHit;

        //rigid = agent.gameObject.gameObject.GetComponent<Rigidbody>();
        //rigid.useGravity = true;

    }

    public override NodeState Execute()
    {
        if(isHit.Value > 0)
        {
            return NodeState.Failure;
        }

        Debug.Log("���");
        animator.enabled = false;
        ChangeLayersRecursively(a, "DeadNPC");
        ChangeTagRecursively(agent.gameObject, "DeadNPC");
        return NodeState.Running;
    }

    public override NodeState Stop()
    {
        ///��� �������� ��ž�� ���� �ʿ䰡 �ֳ�?
        return NodeState.Failure;
    }

    public void ChangeLayersRecursively(Transform parent, string layerName)
    {
        // �θ� ������Ʈ�� ���̾� ����
        parent.gameObject.layer = LayerMask.NameToLayer(layerName);

        // ��� �ڽ� ������Ʈ�� ���� ��������� ���̾� ����
        foreach (Transform child in parent)
        {
            ChangeLayersRecursively(child, layerName);
        }
    }

    public void ChangeTagRecursively(GameObject obj, string tag)
    {
        if (obj != null)
        {
            obj.tag = tag;
            foreach (Transform child in obj.transform)
            {
                ChangeTagRecursively(child.gameObject, tag);
            }
        }
    }
}

public class CoroutineRunner : MonoBehaviour
{
    private static CoroutineRunner _instance;

    public static CoroutineRunner Instance
    {
        get
        {
            if (_instance == null)
            {
                var obj = new GameObject("CoroutineRunner");
                _instance = obj.AddComponent<CoroutineRunner>();
                DontDestroyOnLoad(obj); 
            }
            return _instance;
        }
    }
}