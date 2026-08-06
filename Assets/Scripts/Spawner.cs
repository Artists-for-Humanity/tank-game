using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class EnemySpawnType
{
    public GameObject prefab;
    [NonSerialized]
    public int count = 0;
    public int maxCount;
    public float rarity;
}

public class Spawner : MonoBehaviour
{

    private float timer = 0.0f;
    public float enemySpawnInterval = 1.0f;
    public GameObject[] enemySpawnPositions;

    private GameObject player;

    private int wave = 1;
    private int enemiesLeft;
    private Action<int> onWaveStarted;
    private Action<int> onWaveEnded;

    [SerializeField]
    public EnemySpawnType[] enemySpawnTypes;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        onWaveEnded = OnWaveEnded;
        onWaveStarted = OnWaveStarted;

        enemiesLeft = 10;

        player = GameObject.FindGameObjectWithTag("Player");
    }


    void Update()
    {
        timer += Time.deltaTime;

        if (timer > enemySpawnInterval)
        {
            timer = 0f;
            SpawnEnemy(GetNextEnemy());
            print("sugma");
        }
    }

    EnemySpawnType GetNextEnemy()
    {
        float randomNum = UnityEngine.Random.Range(0f, 1f);
        EnemySpawnType chosenType = null;
        float lowest = math.INFINITY;

        foreach (EnemySpawnType enemySpawnType in enemySpawnTypes)
        {
            if (randomNum <= enemySpawnType.rarity && enemySpawnType.rarity <= lowest)
            {
                chosenType = enemySpawnType;
                lowest = enemySpawnType.rarity;

            }
        }

        return chosenType;
    }
    // Update is called once per frame
    void OnWaveStarted(int wave)
    {

    }

    void OnWaveEnded(int wave)
    {

    }

    void SpawnEnemy(EnemySpawnType enemySpawnType)
    {
        print(enemySpawnType.count.ToString());

        if (enemySpawnType.count >= enemySpawnType.maxCount) { return; }

        enemySpawnType.count++;

        int randomInteger = UnityEngine.Random.Range(0, enemySpawnPositions.Length - 1);

        GameObject newEnemy = Instantiate(enemySpawnType.prefab, enemySpawnPositions[randomInteger].transform.position, Quaternion.identity);
        print("into it");
        newEnemy.GetComponent<EnemyAI>().RefreshStats();
        newEnemy.GetComponent<EnemyAI>().follow = player;
        newEnemy.GetComponent<HealthComponent>().onDied += () =>
        {
            Destroy(newEnemy);

            LevelManager.Instance.AddExperience(500f);
        };


    }
}
