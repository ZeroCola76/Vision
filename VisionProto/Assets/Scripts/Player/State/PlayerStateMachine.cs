using Cinemachine;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(2)]
public class PlayerStateMachine : StateMachine, IListener
{
    public Vector3 velocity;
    public float moveSpeed = 10f;
    public float jumpForce = 2f;
    public float grapplingSpeed = 2.0f;
    public float dashDistance = 10f;
    public float dashDuration = 0.2f;
    public float dashSpeed = 10f;
    public float slideDistance = 15f;
    public float slideSpeed = 3f;
    public float slideDuration = 0.5f;
    public float fallSpeed = 5f;

    public float VPSpeed = 30f;
    public float VPJumeForce = 3f;

    public Transform direction { get; private set; }
    public Transform mainCamera { get; private set; }
    public Transform head { get; private set; }
    public Transform groundCheck;
    public Rigidbody rigid;
    public CapsuleCollider capsuleCollider;
    public BoxCollider boxCollider;
    public InputTest input;
    public Vector3 originPos;
    public Vector3 sitPos;
    public Vector3 slashPoint;

    public PlayerAnimator animator;

    // 마우스를 통해 인식하는 GameObject
    public GameObject targetGameObject;
    // 무기를 장착했니?
    public bool isEquiped;
    // 마우스를 통해 인식하는 Layer
    public int layerMask;
    // Object Tag
    public string objectTag;

    // Player 초기 Spawn을 알려주는 bool
    public bool isSpawn;

    // VP 상태인가요?
    public bool isVPState;

    public string gunName;

    public GameObject VPStateRange;
    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        input = GetComponent<InputTest>();
        head = GameObject.Find("Head").GetComponent<Transform>();
        mainCamera = Camera.main.transform;
        direction = GameObject.Find("Direction").GetComponent<Transform>();
        rigid = GetComponent<Rigidbody>();

        /// Save point와 관련된 공간
        //         {
        //             PlayerInformation playerinfo = new PlayerInformation();
        //             playerinfo = DataManager.Instance.LoadData();
        //             this.transform.localPosition = playerinfo.position;
        //             this.transform.localRotation = playerinfo.rotation;
        //         }


        GameObject vpState = Resources.Load<GameObject>("Player/VP State");
        VPStateRange = vpState;
        VPStateRange = Object.Instantiate(VPStateRange);


        SwitchState(new IdleState(this));

        originPos = /*transform.position - head.localPosition*/new Vector3(0, 0.725f, 0);
        sitPos = new Vector3(0, 0.125f, 0);
        EventManager.Instance.AddEvent(EventType.isEquiped, OnEvent);
        EventManager.Instance.AddEvent(EventType.Hooking, OnEvent);
        isEquiped = false;

        animator = GetComponent<PlayerAnimator>();
        isSpawn = true;
        isVPState = true;
        EventManager.Instance.NotifyEvent(EventType.VPState, isVPState);
    }

    


    /// 여기다가 isEquiped를 추가해야 할 것 같아.
    public void OnEvent(EventType eventType, object param = null)
    {
        switch (eventType)
        {
            case EventType.isEquiped:
                {
                    isEquiped = (bool)param;
                }
                break;
            case EventType.Hooking:
                {
                    if(param != null)
                        gunName = (string)param;
                }
                break;
        }
    }

    public void ObjectInteraction()
    {
        // 1. 마우스에 Ray를 쏜다.
        // 2. layer가 Grappling, Gun이면 각각에 맞는 State로 이동한다.
        Ray cameraRay = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        LayerMask npcDectorLayerMask = ~LayerMask.GetMask("NPC", "DeadNPC");

        RaycastHit hit;

        if (Physics.Raycast(cameraRay, out hit, 1000f, npcDectorLayerMask))
        {
            CinemachineBrain brain = Camera.main.GetComponent<CinemachineBrain>();
            Camera cinemachineCamera = null;

            if (brain != null)
                cinemachineCamera = brain.OutputCamera;

            if (cinemachineCamera != null)
            {
                // camera에서 viewport Point를 가져온다.
                Vector3 viewportPoint = cinemachineCamera.WorldToViewportPoint(hit.transform.position);

                // viewport 에서 Ray를 쏘고 그린다.
                Ray ray = cinemachineCamera.ViewportPointToRay(viewportPoint);

                // Ray 쏜 물체들을 모두 담는다. 이상한 것 까지 다 담는다.
                RaycastHit[] hits = Physics.RaycastAll(ray, 10000f, npcDectorLayerMask);

                if (hits.Length == 0)
                    return;

                System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

                // 순회하면서 Grappling Point나 Grappling에 닿아야 하는데?
                int layer = hits[0].transform.gameObject.layer;
                targetGameObject = hits[0].transform.gameObject;
                objectTag = hits[0].transform.gameObject.tag;
                layerMask = (int)Mathf.Pow(2, layer);
            }
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Vector3 boxSize = new Vector3(transform.lossyScale.x, 1f, transform.lossyScale.z);
        Gizmos.DrawCube(groundCheck.position, boxSize);
    }
}
