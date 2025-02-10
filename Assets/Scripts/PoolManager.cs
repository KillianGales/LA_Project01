/*using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }

    private Dictionary<string, object> pools = new Dictionary<string, object>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void CreatePool(GameObject prefab, int initialSize) 
    {
        string key = prefab.gameObject.name;
        if (!pools.ContainsKey(key))
        {
            pools[key] = new ObjectPool();
            
        }
    }

    public ObjectPool GetObject(GameObject prefab, Vector3 position) 
    {
        string key = prefab.gameObject.name;
        if (pools.ContainsKey(key))
        {
            return ((ObjectPool)pools[key]).GetObject(position);
        }
        return null;
    }

    public void ReturnObject<T>(T obj)
    {
        string key = obj.gameObject.name;
        if (pools.ContainsKey(key))
        {
            ((ObjectPool)pools[key]).ReturnObject(obj);
        }
    }
}*/