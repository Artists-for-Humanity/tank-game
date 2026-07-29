using System;
using System.Data;
using Unity.Mathematics;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    private GameObject player;

    private float respawnTimer = 0.0f;
    private float respawnInterval = 3.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private InputAction openMenuAction;


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
        openMenuAction = InputSystem.actions.FindAction("OpenStats");

        player = GameObject.FindGameObjectWithTag("Player");
        
        player.GetComponent<HealthComponent>().healthChanged = (float oldHealth, float newHealth) =>
        {
            UIManager.UpdateHealthBar(player.GetComponent<HealthComponent>().HealthAsPercentage());
        };

        UIManager.SetGameOverUIEnabled(false);
        player.GetComponent<HealthComponent>().onDied = () =>
        {
            UIManager.SetGameOverUIEnabled(true);
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) { return; }




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
        if (openMenuAction.WasPressedThisFrame())
        {
            UIManager.ToggleUpgradeUI();
        }
    }

    void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


}
