using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using TMPro;
using System.Linq;

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
    [SerializeField]int remainingEnemies, remainingMiniBoss;
    [SerializeField]int round;
    [SerializeField]int EnemiesToSpawn;
    [SerializeField]int baseEnemiesPerRound;
    [SerializeField]float roundEndBuffer;
    [SerializeField] TMP_Text roundText;
    public static SpawnerManager instance;
    private Enemy currEnemy;
    private EEnemytype enemyType;
    private int miniBossToSpawn, TotalEnemiesToSpawn, TotalRoundEnemies;
    public int baseMiniBossPerRound, firstMiniBossRound, typeToSpawn;
    public int miniBossSpawnIndex;
    public List<EnemyType> enType;    
    public const string HIGHEST_ROUND_KEY = "highestRound";
    public int currentHighest;
    public Transform outOfBoundsEnemyPool;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        spawners = GetComponentsInChildren<LineRenderer>();

        //Add it direct in editor
        enemyPool = gameObject.AddComponent<ObjectPool>();
        enemyPool.InitializePool(enemy, 10, outOfBoundsEnemyPool);

        currentHighest = GetHighestRound();

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

        round = GameManager.Instance.startingRound;

        roundText.text = round.ToString();
        StartRound();
       // NextRound();
    }

    /*void cleanStats()
    {
        TotalEnemiesToSpawn = 0;
    }*/

    private IEnumerator SpawnRoutine()
    {
        
        while(TotalEnemiesToSpawn>0)
        {
            SpawnEnemy();
            TotalEnemiesToSpawn--;
            yield return new WaitForSeconds(Random.Range(minSpawnInter, maxSpawnInter));
        }

    }

    private void SpawnRateDecr()
    {
        //refacto en courbe d'anim pour contrôler le spawn rate par round et over time
        if (minSpawnInter > maxSpawnRate)
        {
            minSpawnInter -= spawnIncr;
            maxSpawnInter -= spawnIncr;
        }
    }

    private void SpawnEnemy()
    {

        int index = Random.Range(0, spawnPoints.Count);

        if(miniBossSpawnIndex == -1 && remainingMiniBoss > 0)
        {
            miniBossSpawnIndex = Random.Range(remainingMiniBoss, TotalEnemiesToSpawn);
        }

        //optim plus tard --> manage le pool depuis ici et ne pas get chaque enemy
        GameObject newEnemy = enemyPool.GetObjectFromPool(spawnPoints[index]);
        currEnemy  = newEnemy.GetComponent<Enemy>();
        currEnemy.pool = enemyPool;

        if(miniBossSpawnIndex == TotalEnemiesToSpawn)
        {
 //           Debug.Log("Spawn MiniBoss at index " + miniBossSpawnIndex + "at the index " + TotalEnemiesToSpawn);
            miniBossSpawnIndex = -1;
            remainingMiniBoss--;
            currEnemy.myType = enType[1];
        }
        else 
        {
//            Debug.Log("Spawn Standard enemy at index " + TotalEnemiesToSpawn);
            EnemiesToSpawn--;
            currEnemy.myType = enType[0];
        }
 
    }

    public void EnemyDefeated()
    {
        remainingEnemies--;

        if(remainingEnemies <= 0)
        {
            StartCoroutine(NextRound());
        }
    }

    IEnumerator NextRound()
    {
        round++;
        roundText.text = round.ToString();

        if(round%5 ==0)
        {
            SaveHighestRound();
        }

        yield return new WaitForSeconds(roundEndBuffer);

        Debug.Log("Starting Round : " + round);
        StartRound();
    }

    private void StartRound()
    {
        EnemiesToSpawn  = Mathf.RoundToInt(baseEnemiesPerRound * enemyCountIncr * round);

        if(round >= firstMiniBossRound)
        {
            remainingMiniBoss = Mathf.RoundToInt(baseMiniBossPerRound * round);
        }


        //remainingMiniBoss = miniBossToSpawn;
        TotalEnemiesToSpawn = EnemiesToSpawn + remainingMiniBoss;
        TotalRoundEnemies = EnemiesToSpawn + remainingMiniBoss;
        remainingEnemies = TotalEnemiesToSpawn;

        miniBossSpawnIndex = -1;
        SpawnRateDecr();
        StartCoroutine(SpawnRoutine());

    }

    public void SaveHighestRound()
    {
        int highestRound = PlayerPrefs.GetInt(HIGHEST_ROUND_KEY, 0);

        if(round > highestRound)
        {
            PlayerPrefs.SetInt(HIGHEST_ROUND_KEY, round);
            PlayerPrefs.Save();
            Debug.Log($"New highest round saved: {round}");
        }
    }

    public int GetHighestRound()
    {
        return PlayerPrefs.GetInt(HIGHEST_ROUND_KEY, 0);
    }



}
