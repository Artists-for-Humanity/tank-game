using System.Data;
using Unity.Mathematics;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    private float timer = 0.0f;
    public float enemySpawnInterval = 1.0f;
    public GameObject enemy;
    public Vector3[] enemySpawnPositions;

    private GameObject player;
    private GameOverUI gameOverUI;

    private float respawnTimer = 0.0f;
    private float respawnInterval = 3.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        gameOverUI = GameObject.FindGameObjectWithTag("GameOverUI").GetComponent<GameOverUI>();
        gameOverUI.SetEnabled(false);
        player.GetComponent<HealthComponent>().onDied = () =>
        {
            gameOverUI.SetEnabled(true);
        };
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        

        if (player.GetComponent<HealthComponent>().isDead)
        {
            respawnTimer += Time.deltaTime;

            if (respawnTimer >= respawnInterval)
            {
                respawnTimer = 0.0f;

                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            return;
        }

        if (timer >= enemySpawnInterval)
        {
            timer = 0.0f;
            int randomInteger = UnityEngine.Random.Range(0, enemySpawnPositions.Length - 1);

            GameObject newEnemy = Instantiate(enemy);

            newEnemy.transform.position = enemySpawnPositions[randomInteger];

            EnemyAI enemyAIScript = newEnemy.GetComponent<EnemyAI>();
            enemyAIScript.follow = FindFirstObjectByType<PlayerController>().gameObject;
        }
    }
}
