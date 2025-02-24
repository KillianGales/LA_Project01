using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject prefab;  // The object to pool
    private int poolSize;  // Initial size of the pool
    private string poolName;

    private Queue<GameObject> pool = new Queue<GameObject>();

    public void InitializePool(GameObject prefab, int poolSize)
    {
        if (!prefab) return; 
        this.prefab = prefab;
        this.poolSize = poolSize;
        poolName = prefab.name;

        // Pre-instantiate objects and disable them
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(prefab, transform);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
        return;
    }

    public GameObject GetObjectFromPool(Vector3 position)
    {
        if (!prefab) return null;
        
        GameObject obj;

        if (pool.Count > 0)
        {           
            obj = pool.Dequeue();
            obj.SetActive(true);
            obj.transform.position = position;
            return obj;
        }
        else
        {
            obj = Instantiate(prefab, transform);
            obj.SetActive(true);
            obj.transform.position = position;
            return obj;
        }
    }

    public void ReturnObject(GameObject obj)
    {
        if (!obj) return;
        obj.SetActive(false);
        pool.Enqueue(obj);
        return;
    }
}