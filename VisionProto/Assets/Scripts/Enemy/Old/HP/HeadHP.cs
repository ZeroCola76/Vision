using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���谡 �߸��� �� ����
/// </summary>
public class HeadHP : MonoBehaviour, IDamageable
{
    private EnemyHP enemyHP;
    // Start is called before the first frame update
    void Start()
    {
        enemyHP = GetComponentInParent<EnemyHP>();
    }

    public void Damaged(int damage, Vector3 hitPoint, Vector3 hitNormal, GameObject source)
    {
        enemyHP.TestBehavior.m_Animator.SetBool("Hit", true);
        enemyHP.TestBehavior.m_Animator.SetBool("Attack", false);
        enemyHP.TestBehavior.m_Animator.SetBool("Chase", false);
        //enemyHP.HP -= damage;

        if (enemyHP.HP <= 0)
        {
            enemyHP.eyeRigidbody.useGravity = true;
            enemyHP.TestBehavior.m_Animator.SetTrigger("Death");

            ///���Ӱ͵� �������� ������ ���� �Ǿ���.
            //gameObject.layer = LayerMask.NameToLayer("DeadNPC");
            ///�̰͵� ���д�. ��� �ٲ�
            //Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("NPC"), true);
            ///���д�. �ֳĸ� �ٴڱ��� �հ�...�׷��� �Ǿ����
            ///���̾� ������ �ؾ��Ѵ�.�ٵ� 
            //for (int i = 0; i<TestBehavior.boxcolider.Length; i++)
            //{

            //TestBehavior.boxcolider[i].enabled = false;
            //}
        }
        enemyHP.Hit();
    }

    public void Died()
    {
        //��.
        enemyHP.ChangeTagRecursively(this.gameObject, "DeadNPC");
        enemyHP.ChangeLayersRecursively(transform, "DeadNPC");
    }
}
