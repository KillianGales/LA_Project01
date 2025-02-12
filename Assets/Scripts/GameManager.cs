using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public List<Transform> allTurrets = new List<Transform>();

    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddObject(Transform obj)
    {
        if (!allTurrets.Contains(obj))
        {
            allTurrets.Add(obj);
        }
    }

    public void RemoveObject(Transform obj)
    {
        if (allTurrets.Contains(obj))
        {
            allTurrets.Remove(obj);
        }
    }
}
