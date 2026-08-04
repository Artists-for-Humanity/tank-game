using UnityEngine;
using Unity.Mathematics;
using UnityEngine.InputSystem.Interactions;
using TankGame.Events;
using UnityEditor;
using Mono.Cecil;
using System;

public class LevelManager : MonoBehaviour
{
    private float playerLevel = 0.0f;
    private float playerExperience = 0.0f;

    private const float experienceExponentRate = 0.005f;
    private const float experienceLinearRate = 100f;

    private GameObject UpgradeUI;
    private StatBar healthStatBar;
    private StatBar bulletDamageStatBar;
    private StatBar bulletSpeedStatBar;
    private StatBar firerateStatBar;
    private StatBar vehicleSpeedStatBar;

    private GameObject player;
    public Action onLevelUp;

    public WeaponUpgrade weaponUpgrade;
    public VehicleUpgrade vehicleUpgrade;

    private WeaponUpgrade[] weaponUpgradeBranch;
    public WeaponUpgrade[] gunBranch;
    public WeaponUpgrade[] flameBranch;
    public WeaponUpgrade[] railBranch;
    public WeaponUpgrade[] bigBranch;
    public int currentUpgrade = 0;



    private static LevelManager _instance;
    public static LevelManager Instance { get { return _instance; } }

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
        player = GameObject.FindGameObjectWithTag("Player");
        UpgradeUI = GameObject.Find("UpgradeUI");
        Transform sBackground = UpgradeUI.transform.Find("StatBackground");

        healthStatBar = sBackground.Find("StatBar5").gameObject.GetComponent<StatBar>();
        bulletDamageStatBar = sBackground.Find("StatBar4").gameObject.GetComponent<StatBar>();
        bulletSpeedStatBar = sBackground.Find("StatBar3").gameObject.GetComponent<StatBar>();
        firerateStatBar = sBackground.Find("StatBar2").gameObject.GetComponent<StatBar>();
        vehicleSpeedStatBar = sBackground.Find("StatBar1").gameObject.GetComponent<StatBar>();

        healthStatBar.onValueChanged = (float oldValue, float newValue) =>
        {
            player.GetComponent<PlayerController>().statMultipliers.health = 1f * newValue/2.5f;
            player.GetComponent<PlayerController>().RefreshStats();
        };
        bulletDamageStatBar.onValueChanged = (float oldValue, float newValue) =>
        {
            player.GetComponent<PlayerController>().weaponStatMultipiers.bulletDamage = 1f + newValue/5f;
            player.GetComponent<PlayerController>().RefreshStats();
        };;
        bulletSpeedStatBar.onValueChanged = (float oldValue, float newValue) =>
        {
            player.GetComponent<PlayerController>().weaponStatMultipiers.bulletSpeed = 1f + newValue/10f;
            player.GetComponent<PlayerController>().RefreshStats();
        };;
        firerateStatBar.onValueChanged = (float oldValue, float newValue) =>
        {
            player.GetComponent<PlayerController>().weaponStatMultipiers.firerate = 1f - newValue/20f;
            player.GetComponent<PlayerController>().RefreshStats();
        };;
        vehicleSpeedStatBar.onValueChanged = (float oldValue, float newValue) =>
        {
            player.GetComponent<PlayerController>().statMultipliers.vehicleSpeed = 1f + newValue/10f;
            player.GetComponent<PlayerController>().RefreshStats();
        };;


        onLevelUp += () =>
        {
            if (playerLevel % 5f == 0f)
            {
                weaponUpgradeBranch = gunBranch;
                currentUpgrade = (int)playerLevel / 5 - 1;
                player.GetComponent<PlayerController>().LoadWeapon(player.GetComponent<PlayerController>().weaponSlots[0], weaponUpgradeBranch[currentUpgrade]);
            }
        };
    }

    public float GetPlayerLevelUpRequirement(float level)
    {
        return math.pow((level + 1f) * experienceLinearRate, 1.0f + level * experienceExponentRate);
    }

    public void AddExperience(float experience)
    {
        playerExperience += experience;

        float levelUpRequirement = GetPlayerLevelUpRequirement(playerLevel);

        if (playerExperience >= levelUpRequirement)
        {
            playerExperience = 0.0f;
            playerLevel += 1.0f;

            onLevelUp?.Invoke();
        }

        UIManager.UpdateExperienceBar(playerExperience / levelUpRequirement, playerLevel);
    }
}
