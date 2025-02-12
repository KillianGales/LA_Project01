using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class SpawnerManager : MonoBehaviour
{
    private ObjectPool enemyPool;
    [SerializeField] private float minSpawnInter, maxSpawnInter;
    [SerializeField] private LineRenderer[] spawners;
    [SerializeField] List<Vector3> spawnPoints;
    [SerializeField] private GameObject enemy;

    void Awake()
    {
        spawners = GetComponentsInChildren<LineRenderer>();
        enemyPool = gameObject.AddComponent<ObjectPool>();
        enemyPool.InitializePool(enemy, 20);
    }
    void Start()
    {
        foreach(var spawn in spawners)
        {
            Vector3[] pointArray = new Vector3[spawn.positionCount];

            spawn.GetPositions(pointArray);

            for(int i = 0;i<pointArray.Length; i++)
            { 
                spawnPoints.Add(pointArray[i]);
            }
        }

        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(Random.Range(minSpawnInter, maxSpawnInter));
        }
    }

    private void SpawnEnemy()
    {
        int index = Random.Range(0, spawnPoints.Count);
        GameObject newEnemy = enemyPool.GetObjectFromPool(spawnPoints[index]);
    }
}
