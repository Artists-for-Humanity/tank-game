using System;
using Unity.VisualScripting;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    private float timer = 0.0f;
    public float enemySpawnInterval = 1.0f;
    public GameObject enemy;
    public Vector3[] enemySpawnPositions;

    private GameObject player;

    private float respawnTimer = 0.0f;
    private float respawnInterval = 3.0f;
    private int wave = 1;
    private int enemiesLeft;
    private Action<int> onWaveStarted;
    private Action<int> onWaveEnded;


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
            SpawnEnemy();


        }
    }
    // Update is called once per frame
    void OnWaveStarted(int wave)
    {
        
    }

    void OnWaveEnded(int wave)
    {
        
    }

    void SpawnEnemy()
    {
        int randomInteger = UnityEngine.Random.Range(0, enemySpawnPositions.Length - 1);
       
        GameObject newEnemy = Instantiate(enemy, enemySpawnPositions[randomInteger], Quaternion.identity);
        newEnemy.GetComponent<EnemyAI>().statMultipliers.bulletDamage = 0.05f;
        newEnemy.GetComponent<EnemyAI>().statMultipliers.bulletSpeed = 0.5f;

        newEnemy.GetComponent<EnemyAI>().RefreshStats();
        newEnemy.GetComponent<EnemyAI>().follow = player;
        newEnemy.GetComponent<HealthComponent>().onDied += () =>
        {
            Destroy(newEnemy);

            LevelManager.Instance.AddExperience(500f);
        };


    }
}
