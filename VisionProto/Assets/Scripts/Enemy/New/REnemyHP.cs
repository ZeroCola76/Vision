using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class REnemyHP : MonoBehaviour, IDamageable
{
    public float HP; //µð¹ö±ë¿ë
    private BaseEnemy baseEnemy;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        baseEnemy = GetComponent<BaseEnemy>();
        HP = baseEnemy.HP.Value;
    }

    public void Damaged(int damage, Vector3 hitPoint, Vector3 hitNormal, GameObject source)
    {
        HP -= damage;
        baseEnemy.HP.Value -= damage;
    }

    public void Died()
    {

    }
}
