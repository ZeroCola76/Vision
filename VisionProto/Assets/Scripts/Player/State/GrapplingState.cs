using Cinemachine;
using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;


[DefaultExecutionOrder(-2)]
public class GrapplingState : BaseState, IListener
{
    public GrapplingState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    private Transform gunTip, camera, player;
    private LayerMask objectLayer;
    private LineRenderer lineRenderer;
    private SpringJoint joint;

    private float maxDistance = 1000f;
    private Vector3 grapplePoint;
    private Transform grappleTransform;
    private bool isHooking;
    private Transform direction;
    private GameObject gameObject;
    private float dashSpeed;
    private bool dashKeyDown;
    private float totalTime;

    private bool none;
    private bool isActive;

    public override void Enter()
    {
        stateMachine.ObjectInteraction();

        // LayerMask�� �����Ѵ�.
        objectLayer = LayerMask.GetMask("Object");

        stateMachine.velocity.y = Physics.gravity.y;
        dashSpeed = 1.0f;
        // camera, gunTip, Player�� Transform�� �޾ƿ´�.
        camera = GameObject.Find("Virtual Camera").GetComponent<Transform>();
        gunTip = GameObject.Find("GunTip").GetComponent<Transform>();
        player = GameObject.Find("Player").GetComponent<Transform>();

        // gunTip�� �ִ� LineRenderer�� �޾ƿ´�.
        lineRenderer = GameObject.Find("GunTip").GetComponent<LineRenderer>();

        // Player�� SpringJoint�� �߰��Ѵ�.

        joint = player.gameObject.AddComponent<SpringJoint>();

        // Player�� NPC�� �浹���� �ʰ� �����Ѵ�.
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("NPC"), true);
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Gun"), LayerMask.NameToLayer("NPC"), true);

        StartGrappling();
        DrawRope();

        gameObject = new GameObject();
        direction = gameObject.transform;

        EventManager.Instance.AddEvent(EventType.Hooking, OnEvent);
    }

    public override void Tick()
    {
        DrawRope();

        //if (Input.GetKeyDown(KeyCode.F))

        if (Input.GetMouseButtonDown(1))
        {
            StartGrappling();
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            stateMachine.SwitchState(new SlashState(stateMachine));
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            dashSpeed = 5.0f;
            dashKeyDown = true;
            totalTime = 0.0f;
        }

        //if (Input.GetKeyUp(KeyCode.F))
        //         if (Input.GetMouseButtonUp(1))
        //         {
        //             StopGrappling();
        //             isHooking = true;
        //             none = false;
        //         }


        if (dashKeyDown)
        {
            totalTime += Time.deltaTime;
            if (totalTime > 0.5f)
            {
                dashKeyDown = false;
                dashSpeed = 1.0f;
            }
        }

        RopeRebound();

        // �Ӹ��� ����� �� ���� isGrounded�� true�� �ȴ� => ����� ��� ������ �ٲ�� �� ����.
        if (stateMachine.input.isGrounded && isHooking)
            stateMachine.SwitchState(new FallState(stateMachine));

        // jump grappling�� �� �ٸ���.
        if (none && stateMachine.input.isGrounded)
        {
            stateMachine.SwitchState(new FallState(stateMachine));
        }

    }

    public override void FixedTick()
    {
        if (isHooking || none)
        {
            stateMachine.rigid.AddForce(new Vector3(0, Physics.gravity.y * 10f, 0));
        }
    }

    public override void Exit()
    {
        isHooking = false;
        none = false;
        Object.Destroy(gameObject);
        EventManager.Instance.RemoveEvent(EventType.Hooking);
        stateMachine.targetGameObject = null;
    }



    void StartGrappling()
    {
        if (isActive)
            return;

        RaycastHit hit;
        isActive = true;

        // ī�޶� ��ġ, �� ��ġ, ���, �ִ� �Ÿ�, � LayerMask��?
        if (Physics.Raycast(origin: camera.position, direction: camera.forward, out hit, maxDistance, objectLayer))
        {
            isHooking = false;

            if (joint == null)
                joint = player.gameObject.AddComponent<SpringJoint>();

            grappleTransform = hit.transform;
            grapplePoint = hit.point;


            float distanceFromPoint = Vector3.Distance(a: player.position, b: grapplePoint);

            // ���� ��¼�� grappling point�� �ɷ��� �ϴµ� �ٸ����� �ɸ���
            IsRayTarget(grappleTransform, distanceFromPoint);

            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;


            // Grapple point �Ÿ��� ������ �� �ִ�.
            /// ���� ��ġ�� ������ �� �ִ�.
            joint.maxDistance = distanceFromPoint * 0.2f;
            joint.minDistance = distanceFromPoint * 0.1f;

            // �̰� ��Ȳ�� �°� �����ϸ�ȴ�.
            /// �ӵ��� ������ �� �ִ�?
            // joint.spring = 3f;
            // joint.massScale = 1.0f;
            joint.damper = 1f;


            // position Count�� �̿��ؼ� lineRenderer�� �׸���.
            lineRenderer.positionCount = 2;
            SoundManager.Instance.PlayEffectSound(SFX.Grappling_Start, stateMachine.transform);

            stateMachine.animator.OnGrappling();
        }
        else
        {
            Object.Destroy(joint);
            none = true;
        }


    }

    void StopGrappling()
    {
        lineRenderer.positionCount = 0;
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("NPC"), false);
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Gun"), LayerMask.NameToLayer("NPC"), false);
        SoundManager.Instance.PlayEffectSound(SFX.Grappling_End, stateMachine.transform);
        isActive = false;

        // Add Component ����
        if (joint != null)
            Object.Destroy(joint);
    }

    void DrawRope()
    {
        if (!joint)
            return;

        if (joint.connectedAnchor != grapplePoint)
            return;

        // �׸� ��ġ gun Tip�� ��ġ, grapplePoint ��ġ
        lineRenderer.SetPosition(index: 0, gunTip.position);
        lineRenderer.SetPosition(index: 1, grapplePoint);
    }

    void RopeRebound()
    {
        // ����Ű�� �޾ƿ´�.
        float horizon = stateMachine.input.hAxis;
        float vertical = stateMachine.input.vAxis;

        // ���� Ű�� ���� �����δ�. AddForce�� ���ش�.

        // ȸ���� �޾ƿ��� �ʹ�.
        direction.rotation = stateMachine.direction.rotation;

        // �Ź� ���ö����� 0,0,0���� �ʱ�ȭ ���ش�.
        direction.position = Vector3.zero;

        /// ����
        if (horizon == -1)
            direction.position += -Vector3.right.normalized * dashSpeed /** stateMachine.grapplingSpee*/;
        /// ������
        if (horizon == 1)
            direction.position += Vector3.right.normalized * dashSpeed /** stateMachine.grapplingSpeed*/;
        /// �Ʒ�
        if (vertical == -1)
            direction.position += -Vector3.forward.normalized * dashSpeed /** stateMachine.grapplingSpeed*/;
        /// ��
        if (vertical == 1)
            direction.position += Vector3.forward.normalized * dashSpeed /** stateMachine.grapplingSpeed*/;

        // direction ���� �ް� Addforce�� ���ִµ�, SRT -> Rotaion * Transpose�� ���ָ� �ȴ�. 0.1 �����ָ� �ʹ� ���� �����.
        stateMachine.rigid.AddForce(direction.rotation * direction.position * 0.1f, ForceMode.Impulse);
    }

    public void OnEvent(EventType eventType, object param = null)
    {
        switch (eventType)
        {
            case EventType.Hooking:
                {
                    StopGrappling();
                    isHooking = true;
                    none = false;
                }
                break;
        }
    }

    // LayerMask ���� �̿��ϰ� �;� �׷��� �� �� ������
    void IsRayTarget(Transform _transform, float distance)
    {
        CinemachineBrain brain = Camera.main.GetComponent<CinemachineBrain>();
        Camera camera = null;

        if (brain != null)
            camera = brain.OutputCamera;

        if (camera != null)
        {
            // camera���� viewport Point�� �����´�.
            Vector3 viewportPoint = camera.WorldToViewportPoint(_transform.position);

            // viewport ���� Ray�� ��� �׸���.
            Ray ray = camera.ViewportPointToRay(viewportPoint);

            // Layer Mask�� ����� �Գ�

            LayerMask enemyLayerMask = ~LayerMask.GetMask("NPC");

            // Ray �� ��ü���� ��� ��´�.
            RaycastHit[] hits = Physics.RaycastAll(ray, 1000f, enemyLayerMask);

            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            // �ƴ� ���ڱ� Layer 13, 11�� �ߵǴ��� ���ڱ� 2^n���� �ǰ� ����?

            // ù��° Ray ���µ� layer�� Grappling�� �ƴϸ� false�� �����Ѵ�.
            // ���⼭ Layer ��ſ� tag�� �ϸ� Layer ������ ���� �� ���� �� ����.
            string targetTag = default;

            foreach (RaycastHit hit in hits)
            {
                targetTag = hit.transform.gameObject.tag;

                if (targetTag == "GrapplingPoint")
                    grapplePoint = hit.transform.position;
            }
        }
    }
}
