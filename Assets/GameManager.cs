using System.Data;
using Unity.Mathematics;
using UnityEditor.SearchService;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    private float timer = 0.0f;
    private float enemySpawnInterval = 5.0f;
    public GameObject enemy;
    public Vector3[] enemySpawnPositions;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        
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
