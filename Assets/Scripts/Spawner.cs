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

    private int wave = 1;
    private Action<int> onWaveStarted;
    private Action<int> onWaveEnded;

    [SerializeField]
    public EnemySpawnType[] enemySpawnTypes;

    public float waveLengthSeconds = 1f;
    private float waveEndTimer = 0f;
    private int currentEnemySpawning = 0;
    int amount = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        onWaveEnded = OnWaveEnded;
        onWaveStarted = OnWaveStarted;

        player = GameObject.FindGameObjectWithTag("Player");
        amount = wave * 2;
    }

    int GetTotalEnemies(int wave)
    {
        return wave * 2 + wave / 5 + wave / 15 + wave / 20 + wave / 40;
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
                        amount = wave * 2;
                        break;
                    case 1:
                        amount = wave / 5;
                        break;
                    case 2:
                        amount = wave / 20;
                        break;
                    case 3:
                        amount = wave / 15;
                        break;
                    case 4:
                        amount = wave / 40;
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
            print("gooo");
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


        if (isWarship)
        {
            print("ee");
            Transform seaPositions = GameObject.Find("SeaPositions").transform;

            int randomInteger = UnityEngine.Random.Range(0, seaPositions.childCount - 1);
            GameObject newEnemy = Instantiate(enemySpawnType.prefab, seaPositions.GetChild(randomInteger).transform.position, Quaternion.identity);
        }
        else
        {
            int randomInteger = UnityEngine.Random.Range(0, enemySpawnPositions.Length - 1);
            GameObject newEnemy = Instantiate(enemySpawnType.prefab, enemySpawnPositions[randomInteger].transform.position, Quaternion.identity);
        }

        return true;
    }
}
