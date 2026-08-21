using System;
using UnityEngine;

[Serializable]
public class EnemySpawnType
{
    public GameObject prefab;
    [NonSerialized]
    public int count = 0;
    public int maxCount;
}

public class Spawner : MonoBehaviour
{

    private float timer = 0.0f;
    public float enemySpawnInterval = 1.0f;
    public GameObject[] enemySpawnPositions;


    private GameObject player;

    public int wave = 1;
    private Action<int> onWaveStarted;
    private Action<int> onWaveEnded;

    [SerializeField]
    public EnemySpawnType[] enemySpawnTypes;

    public float waveLengthSeconds = 1f;
    private float waveEndTimer = 0f;
    private int currentEnemySpawning = 0;
    int amount = 0;

    private static Spawner _instance;
    public static Spawner Instance { get { return _instance; } }
    

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        onWaveEnded = OnWaveEnded;
        onWaveStarted = OnWaveStarted;

        player = GameObject.FindGameObjectWithTag("Player");
        amount = wave * 2;
    }

    int GetTotalEnemies(int wave)
    {
        return wave + wave / 5 + wave / 10 + wave / 15 + wave / 20;
    }
    void Update()
    {
        timer += Time.deltaTime;
        waveEndTimer += Time.deltaTime;


        if (timer > enemySpawnInterval && currentEnemySpawning != -1)
        {
            timer = 0f;

            if (amount <= 0)
            {
                currentEnemySpawning++;
                if (currentEnemySpawning >= enemySpawnTypes.Length)
                {
                    currentEnemySpawning = -1;
                }

                switch (currentEnemySpawning)
                {
                    case -1:
                        break;
                    case 0:
                        amount = wave;
                        break;
                    case 1:
                        amount = wave / 5;
                        break;
                    case 2:
                        amount = wave / 15;
                        break;
                    case 3:
                        amount = wave / 10;
                        break;
                    case 4:
                        amount = wave / 20;
                        break;
                }
            }
            else
            {
                TrySpawnEnemy(enemySpawnTypes[currentEnemySpawning], currentEnemySpawning == 4);
                amount--;
            }
        }

        if (waveEndTimer > waveLengthSeconds)
        {
            waveEndTimer = 0f;
            wave++;
            UIManager.UpdateWaveUI(wave);

            onWaveEnded.Invoke(wave);
            onWaveStarted.Invoke(wave);

            enemySpawnInterval = waveLengthSeconds * 0.33f / (float)GetTotalEnemies(wave);
            currentEnemySpawning = 0;
            amount = wave * 2;
        }
    }

    // Update is called once per frame
    void OnWaveStarted(int wave)
    {

    }

    void OnWaveEnded(int wave)
    {

    }

    bool TrySpawnEnemy(EnemySpawnType enemySpawnType, bool isWarship)
    {
        if (enemySpawnType.count >= enemySpawnType.maxCount) { return false; }

        enemySpawnType.count++;

        Vector3 spawnPosition = Vector3.zero;


        if (isWarship)
        {
            Transform seaPositions = GameObject.Find("SeaPositions").transform;

            int randomInteger = UnityEngine.Random.Range(0, seaPositions.childCount - 1);
            spawnPosition = seaPositions.GetChild(randomInteger).transform.position;
        }
        else
        {
            int randomInteger = UnityEngine.Random.Range(0, enemySpawnPositions.Length - 1);
            spawnPosition = enemySpawnPositions[randomInteger].transform.position;
        }

        GameObject newEnemy = Instantiate(enemySpawnType.prefab, spawnPosition, Quaternion.identity);
        newEnemy.GetComponent<HealthComponent>().onDied += () =>
        {
            enemySpawnType.count--;
        };

        return true;
    }
}
