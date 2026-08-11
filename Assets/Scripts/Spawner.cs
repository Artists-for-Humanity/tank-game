using System;
using System.Collections.Generic;
using System.Linq;
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
    public int spawnsAfterWave = 0;
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

    public float waveLengthSeconds = 1f;
    private float waveEndTimer = 0f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        onWaveEnded = OnWaveEnded;
        onWaveStarted = OnWaveStarted;

        enemiesLeft = CalculateNumEnemies(wave);

        player = GameObject.FindGameObjectWithTag("Player");
    }

    int CalculateNumEnemies(int wave)
    {
        return (wave) * 2;
    }


    void Update()
    {
        timer += Time.deltaTime;
        waveEndTimer += Time.deltaTime;

        if (timer > enemySpawnInterval && enemiesLeft >= 0)
        {
            timer = 0f;
            if (TrySpawnEnemy(GetNextEnemy()))
            {
                enemiesLeft--;
            }
        }

        if (waveEndTimer > waveLengthSeconds)
        {
            print("gooo");
            waveEndTimer = 0f;
            wave++;
            UIManager.UpdateWaveUI(wave);

            enemiesLeft += CalculateNumEnemies(wave);

            onWaveEnded.Invoke(wave);
            onWaveStarted.Invoke(wave);
        }
    }

    EnemySpawnType GetNextEnemy()
    {
        float randomNum = UnityEngine.Random.Range(0f, 1f);
        EnemySpawnType chosenType = enemySpawnTypes[0];
        float lowest = math.INFINITY;

        List<EnemySpawnType> possibleSpawns = new List<EnemySpawnType>();

        foreach (EnemySpawnType spawn in enemySpawnTypes)
        {
            if (spawn.spawnsAfterWave <= wave && spawn.count < spawn.maxCount)
            {
                possibleSpawns.Add(spawn);
            }
        }


        for (int i = 0; i < possibleSpawns.Count; i++)
        {
            EnemySpawnType enemySpawnType = possibleSpawns[i];

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

    bool TrySpawnEnemy(EnemySpawnType enemySpawnType)
    {
        if (enemySpawnType.count >= enemySpawnType.maxCount) { return false; }

        enemySpawnType.count++;

        int randomInteger = UnityEngine.Random.Range(0, enemySpawnPositions.Length - 1);

        GameObject newEnemy = Instantiate(enemySpawnType.prefab, enemySpawnPositions[randomInteger].transform.position, Quaternion.identity);

        return true;
    }
}
