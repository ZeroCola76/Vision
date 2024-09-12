using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Transform targetPlayer; 

    public float moveSpeed = 400f;   //=>

    private Rigidbody rb;
    Vector3 direction;
    Vector3 moveDirection;
    public float inaccuracyAngle = 500000f;

    public bool isPistols = false;
    public bool isAssault = false;
    public bool isShot = false;
    public bool isSniping = false;

    float distance;
    EnemyController enemyController;
    private TrailRenderer tr;

    private void Awake()
    {
        if(isPistols)
        {
            moveSpeed = 400f;
            inaccuracyAngle = 5f;
        }
        if (isAssault)
        {
            moveSpeed = 300f;
            inaccuracyAngle = 5f;
        }
        if (isShot)
        {
            moveSpeed = 500f;
            inaccuracyAngle = 10f;
        }
        if (isSniping)
        {
            moveSpeed = 900f;
            inaccuracyAngle =5f;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        tr = GetComponent<TrailRenderer>();
        enemyController = FindObjectOfType<EnemyController>();
    }

    private void FixedUpdate()
    {
        if (targetPlayer != null)
        {
            rb.velocity = moveDirection * moveSpeed;

        }
        else
        {
            // ����ó��: ��ǥ�� ���� ��, �Ѿ��� �����ϰų� �ٸ� ���� ����
            Destroy(gameObject); // ����
        }
    }

    void OnDisable()
    {
        if (tr == null) return;
        tr.Clear();
    }

    //��ġ ������ �ϰ� �ٲ���� 
    Vector3 ApplyInaccuracy(Vector3 direction, float angle)
    {
        Quaternion randomRotation = Quaternion.Euler(
            Random.Range(-angle, angle),
            Random.Range(-angle, angle),
            Random.Range(-angle, angle)
        );

        // ���� ���⿡ ������ ȸ�� ����
        return randomRotation * direction;
    }

    private void OnTriggerEnter(Collider collider)
    {
        ///������� ����� �ڵ带 ���⼭ ȣ���ϰ�
        ///����� ������°� �ƴ϶�... 
        IDamageable damageable = collider.gameObject.GetComponent<IDamageable>();
        if (damageable != null)
        {
            if (isShot)
            {
                if (enemyController.isFarSec)
                {
                    damageable.Damaged(60, transform.position, transform.position, this.gameObject);

                }
                else if (enemyController.isFarThr)
                {
                    damageable.Damaged(40, transform.position, transform.position, this.gameObject);

                }
                else
                {

                    damageable.Damaged(20, transform.position, transform.position, this.gameObject);
                }
            }
            ///���߿� ���� �ٽ� ����鼭 ���� �ڵ� ����
            else
            {

                damageable.Damaged(1, transform.position, transform.position, this.gameObject);
            }
        }


    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Floor") || collision.gameObject.CompareTag("Player")
            || collision.gameObject.CompareTag("Door") || collision.gameObject.CompareTag("Grappling") || collision.gameObject.CompareTag("GrapplingPoint"))
        {
            gameObject.SetActive(false);
        }
    }

    void Pistol()
    {
        if (targetPlayer != null)
        {
            distance = Vector3.Distance(transform.position, targetPlayer.transform.position);

            if (distance <= 10)
            {
                inaccuracyAngle = 0;
            }
            else if(distance > 10 && distance <= 30)
            {
                inaccuracyAngle = 3;
            }
            else
            {
                inaccuracyAngle = 5;
            }

        }
    }

    void Assult()
    {
        if (targetPlayer != null)
        {
            distance = Vector3.Distance(transform.position, targetPlayer.transform.position);

            if (distance <= 10)
            {
                inaccuracyAngle = 0;
            }
            else if (distance > 10 && distance <= 30)
            {
                inaccuracyAngle = 3;
            }
            else if (distance > 30 && distance <= 50)
            {
                inaccuracyAngle = 5;
            }
            else
            {
                inaccuracyAngle = 6;
            }

        }
    }

    void Sinping()
    {
        if (targetPlayer != null)
        {
            distance = Vector3.Distance(transform.position, targetPlayer.transform.position);

            if (distance <= 100)
            {
                inaccuracyAngle = 3;
            }
            else
            {
                inaccuracyAngle = 5;
            }

        }
    }

    public void SetUp(Transform pos, Transform target)
    {
        gameObject.transform.position = pos.position;
        //Debug.Log("������ " + target.transform.position);
        targetPlayer = target;
        direction = (targetPlayer.transform.position - transform.position).normalized;
        if (isPistols)
        {
            Pistol();
        }
        if (isAssault)
        {
            Assult();
        }
        if (isSniping)
        {
            Sinping();
        }
        //transform.LookAt(targetPlayer);
        moveDirection = ApplyInaccuracy(direction.normalized, inaccuracyAngle);
  
    }
}
