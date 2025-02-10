using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    private GameObject prefab;  // The object to pool
    private int poolSize;  // Initial size of the pool
    private string poolName;
    int numInst = 0; 

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
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
        return;
    }

    public GameObject GetObjectFromPool(Vector3 position)
    {
        if (!prefab) return null;
        
        GameObject obj;

       if (pool.Count <= 0)  return null;
           
        obj = pool.Dequeue();
        obj.SetActive(true);
        obj.transform.position = position;
        numInst += 1;
        //Debug.Log( numInst + "Have Been instantiated");

        return obj;

       // else
       // {
            //Debug.Log("PoolEmpty");
            //GameObject newObj = Instantiate(prefab);
            //pool.Append(newObj);
          
            //obj = Instantiate(prefab);  // Create a new object if pool is empty
      //  }
/*
        obj.SetActive(true);
        obj.transform.position = position;
        return obj;*/
    }

    public void ReturnObject(GameObject obj)
    {
        if (!obj) return;
        obj.SetActive(false);
        pool.Enqueue(obj);
        return;
    }
}