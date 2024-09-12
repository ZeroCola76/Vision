using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class InputTest : MonoBehaviour
{
    public float hAxis;
    public float vAxis;

    public bool isSit;
    public bool isGrounded;
    public bool isOnSlope;
    public bool isJump;
    public bool isDashed;

    public int dashIndex = 0;

    public float playerScale;

    public bool dashDamage;
    public bool slashDamage;

    private void Start()
    {
        playerScale = this.transform.localScale.y; 

        
    }

    private void Update()
    {

        GetInput();
        CheckGround();
        EventManager.Instance.NotifyEvent(EventType.PlayerPosition, this.transform.position);

    }


    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
    }

    public void CheckGround()
    {
        RaycastHit hit;

        LayerMask playerLayerMask = ~LayerMask.GetMask("Default", "NPC");
        isGrounded = Physics.Raycast(transform.position, Vector3.down, out hit, 10f, playerLayerMask);
        //Debug.DrawRay(transform.position, Vector3.down * hit.distance, Color.red);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (dashDamage)
        {
            if (collision.gameObject.CompareTag("NPC"))
            {
                Debug.Log("돌진 상태 충돌");
                IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.Damaged(50, transform.position, transform.position, this.gameObject);
                }
            }
        } 

        if (slashDamage)
        {
            if(collision.gameObject.CompareTag("NPC"))
            {
                Debug.Log("내려찍기 충돌");
                IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.Damaged(10, transform.position, transform.position, this.gameObject);
                }
            }
        }
    }
}