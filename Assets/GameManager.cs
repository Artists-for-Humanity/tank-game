using System;
using System.Data;
using Unity.Mathematics;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    private float timer = 0.0f;
    public float enemySpawnInterval = 1.0f;
    public GameObject enemy;
    public Vector3[] enemySpawnPositions;

    private GameObject player;
    private GameObject gameOverUI;

    private float respawnTimer = 0.0f;
    private float respawnInterval = 3.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

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

    void Start()
    {
        InitializeGame();
    }

    void InitializeGame()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        gameOverUI = GameObject.Find("GameOverUI");

        player.GetComponent<HealthComponent>().healthChanged = (float oldHealth, float newHealth) =>
        {
            CombatUIManager.UpdateHealthBar(player.GetComponent<HealthComponent>().HealthAsPercentage());
        };

        gameOverUI.GetComponent<Canvas>().enabled = false;
        player.GetComponent<HealthComponent>().onDied = () =>
        {
            gameOverUI.GetComponent<Canvas>().enabled = true;
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) { return; }

        timer += Time.deltaTime;


        if (player.GetComponent<HealthComponent>().isDead)
        {
            respawnTimer += Time.deltaTime;

            if (respawnTimer >= respawnInterval)
            {
                respawnTimer = 0.0f;

                RestartGame();
            }
            return;
        }

        if (timer >= enemySpawnInterval)
        {
            timer = 0.0f;

            SpawnEnemy();
        }
    }

    void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void SpawnEnemy()
    {
        int randomInteger = UnityEngine.Random.Range(0, enemySpawnPositions.Length - 1);

        GameObject newEnemy = Instantiate(enemy);
        newEnemy.transform.position = enemySpawnPositions[randomInteger];

        newEnemy.GetComponent<EnemyAI>().follow = player;
        newEnemy.GetComponent<HealthComponent>().onDied += () =>
        {
            Destroy(newEnemy);

            LevelManager.Instance.AddExperience(50f);
        };
    }
}
