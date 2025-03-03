using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using TMPro;

public class SpawnerManager : MonoBehaviour
{
    private ObjectPool enemyPool;
    [SerializeField] private float minSpawnInter, maxSpawnInter;
    [SerializeField] private LineRenderer[] spawners;
    [SerializeField] List<Vector3> spawnPoints;
    [SerializeField] private GameObject enemy;
    public AnimationCurve spawnCurve;
    public float spawnIncr, maxSpawnRate, enemyCountIncr;
    [Header("Spawner Settings")]
    [SerializeField]int remainingEnemies;
    [SerializeField]int round;
    [SerializeField]int EnemiesToSpawn;
    [SerializeField]int baseEnemiesPerRound;
    [SerializeField]float roundEndBuffer;
    [SerializeField] TMP_Text roundText;
    private int actualEnemies;

    public static SpawnerManager instance;

    void Awake()
    {
        instance = this;
        spawners = GetComponentsInChildren<LineRenderer>();
        enemyPool = gameObject.AddComponent<ObjectPool>();
        enemyPool.InitializePool(enemy, 10);
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

        StartRound();
       // StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        
        while(EnemiesToSpawn>0)
        {
            SpawnEnemy();
            EnemiesToSpawn--;
            actualEnemies += 1;
            yield return new WaitForSeconds(Random.Range(minSpawnInter, maxSpawnInter));
        }

    }

    private void SpawnRateDecr()
    {
        if (minSpawnInter > maxSpawnRate)
        {
            minSpawnInter -= spawnIncr;
            maxSpawnInter -= spawnIncr;
            //Debug.Log("Spawn rate is between" + minSpawnInter + " and " + maxSpawnInter);
        }
    }

    private void SpawnEnemy()
    {

        int index = Random.Range(0, spawnPoints.Count);
        GameObject newEnemy = enemyPool.GetObjectFromPool(spawnPoints[index]);
        newEnemy.GetComponent<Enemy>().pool = enemyPool;

 
    }

    void Update()
    {

    }

    public void EnemyDefeated()
    {
       // if(remainingEnemies > 0)
       // {
            remainingEnemies-=1;
            
       // }

        if(remainingEnemies <= 0)
        {
            StartCoroutine(NextRound());
        }
    }


    IEnumerator NextRound()
    {
        yield return new WaitForSeconds(roundEndBuffer);
        round++;
        Debug.Log("Starting Round : " + round);
        StartRound();
    }

    private void StartRound()
    {
        EnemiesToSpawn  = Mathf.RoundToInt(baseEnemiesPerRound * enemyCountIncr * round);
        remainingEnemies = EnemiesToSpawn;
        roundText.text = round.ToString();
        actualEnemies = 0;
        SpawnRateDecr();
        StartCoroutine(SpawnRoutine());

    }
}
