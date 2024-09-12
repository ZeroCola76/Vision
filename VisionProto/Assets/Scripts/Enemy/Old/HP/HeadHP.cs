using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 설계가 잘못된 것 같음
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

            ///ㅇㅣ것도 실패지만 성공의 길이 되었다.
            //gameObject.layer = LayerMask.NameToLayer("DeadNPC");
            ///이것도 실패다. 모두 바뀜
            //Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("NPC"), true);
            ///실패다. 왜냐면 바닥까지 뚫고...그렇게 되어버림
            ///레이어 변경을 해야한다.근데 
            //for (int i = 0; i<TestBehavior.boxcolider.Length; i++)
            //{

            //TestBehavior.boxcolider[i].enabled = false;
            //}
        }
        enemyHP.Hit();
    }

    public void Died()
    {
        //흠.
        enemyHP.ChangeTagRecursively(this.gameObject, "DeadNPC");
        enemyHP.ChangeLayersRecursively(transform, "DeadNPC");
    }
}
