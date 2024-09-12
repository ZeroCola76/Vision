using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �迹���� �ۼ�
/// </summary>
public class PoolManager : MonoBehaviour
{
    // Ǯ���� �����հ� �ʱ� Ǯ ũ�⸦ �����ϱ� ���� ����ü
    [System.Serializable]
    public struct Pool
    {
        public string tag;
        public GameObject prefab;
        public int initialSize;
    }

    // Ǯ���� ������ ���
    public List<Pool> pools;
    private Dictionary<string, List<GameObject>> poolDictionary;
    public static PoolManager Instance;

    private void Awake()
    {
        Instance = this;

        poolDictionary = new Dictionary<string, List<GameObject>>();

        foreach (Pool pool in pools)
        {
            List<GameObject> objectPool = new List<GameObject>();

            for (int i = 0; i < pool.initialSize; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Add(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject GetPooledObject(string tag)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning(tag + " �� ����");
            return null;
        }

        foreach (GameObject obj in poolDictionary[tag])
        {
            if (!obj.activeSelf)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        foreach (Pool pool in pools)
        {
            if (pool.tag == tag)
            {
                GameObject newObj = Instantiate(pool.prefab);
                poolDictionary[tag].Add(newObj);
                newObj.SetActive(true);
                return newObj;
            }
        }

        return null;
    }

    public GameObject GetPooledObject(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning(tag + " �� ����");
            return null;
        }

        foreach (GameObject obj in poolDictionary[tag])
        {
            if (!obj.activeSelf)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        foreach (Pool pool in pools)
        {
            if (pool.tag == tag)
            {
                GameObject newObj = Instantiate(pool.prefab, position, rotation);
                poolDictionary[tag].Add(newObj);
                newObj.SetActive(true);
                return newObj;
            }
        }

        return null;
    }


    public void ReturnToPool(GameObject obj, string tag)
    {
        if (poolDictionary.ContainsKey(tag) && poolDictionary[tag].Contains(obj))
        {
            obj.SetActive(false);
        }
        else
        {
            Destroy(obj);
        }
    }
}
