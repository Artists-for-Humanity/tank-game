using UnityEngine;
using Unity.Mathematics;
using UnityEngine.InputSystem.Interactions;
using TankGame.Events;
using UnityEditor;
using Mono.Cecil;
using System;
using UnityEngine.InputSystem;

public class LevelManager : MonoBehaviour
{
    private float playerLevel = 0.0f;
    private float playerExperience = 0.0f;

    private const float experienceExponentRate = 0.001f;
    private const float experienceLinearRate = 100f;

    private GameObject UpgradeUI;
    private StatBar healthStatBar;
    private StatBar bulletDamageStatBar;
    private StatBar bulletSpeedStatBar;
    private StatBar firerateStatBar;
    private StatBar vehicleSpeedStatBar;
    public int statPoints = 0;
    private GameObject player;
    public Action onLevelUp;

    public WeaponUpgrade weaponUpgrade;
    public VehicleUpgrade vehicleUpgrade;

    private WeaponUpgrade[] weaponUpgradeBranch;
    public WeaponUpgrade[] gunBranch;
    public WeaponUpgrade[] flameBranch;
    public WeaponUpgrade[] railBranch;
    public WeaponUpgrade[] bigBranch;

    public VehicleUpgrade[] vehicleUpgrades;
    public int currentUpgrade = 0;



    private static LevelManager _instance;
    public static LevelManager Instance { get { return _instance; } }
    private InputAction upgradeSelectAction;


    bool upgradeLocked = false;

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
            player.GetComponent<PlayerController>().statMultipliers.health = 1f * newValue / 2.5f;
            player.GetComponent<PlayerController>().RefreshStats();
        };
        bulletDamageStatBar.onValueChanged = (float oldValue, float newValue) =>
        {
            player.GetComponent<PlayerController>().weaponStatMultipiers.bulletDamage = 1f + newValue / 5f;
            player.GetComponent<PlayerController>().RefreshStats();
        }; ;
        bulletSpeedStatBar.onValueChanged = (float oldValue, float newValue) =>
        {
            player.GetComponent<PlayerController>().weaponStatMultipiers.bulletSpeed = 1f + newValue / 10f;
            player.GetComponent<PlayerController>().RefreshStats();
        }; ;
        firerateStatBar.onValueChanged = (float oldValue, float newValue) =>
        {
            player.GetComponent<PlayerController>().weaponStatMultipiers.firerate = 1f - newValue / 20f;
            player.GetComponent<PlayerController>().RefreshStats();
        }; ;
        vehicleSpeedStatBar.onValueChanged = (float oldValue, float newValue) =>
        {
            player.GetComponent<PlayerController>().statMultipliers.vehicleSpeed = 1f + newValue / 10f;
            player.GetComponent<PlayerController>().RefreshStats();
        };

        upgradeSelectAction = InputSystem.actions.FindAction("Select");


        UIManager.SetUpgradeUIEnabled(false);

        UIManager.SetVehicleUpgradeUIEnabled(true);

        Action<InputAction.CallbackContext> fn = null;
        fn = (InputAction.CallbackContext callbackContext) =>
        {
            int selection = (int)upgradeSelectAction.ReadValue<float>();
            player.GetComponent<PlayerController>().LoadVehicle(vehicleUpgrades[selection - 1]);

            UIManager.SetVehicleUpgradeUIEnabled(false);
            
            upgradeSelectAction.started -= fn;
        };
        upgradeSelectAction.started += fn;

        onLevelUp += () =>
        {
            if (playerLevel == 5 && !upgradeLocked)
            {
                UIManager.SetUpgradeUIEnabled(true);

                Action<InputAction.CallbackContext> func = null;
                func = (InputAction.CallbackContext callbackContext) =>
                {
                    int selection = (int)upgradeSelectAction.ReadValue<float>();
                    switch (selection)
                    {
                        case 1:
                            weaponUpgradeBranch = gunBranch;
                            break;
                        case 2:
                            weaponUpgradeBranch = bigBranch;
                            break;
                        case 3:
                            weaponUpgradeBranch = railBranch;
                            break;
                        case 4:
                            weaponUpgradeBranch = flameBranch;
                            break;
                    }

                    UIManager.SetUpgradeUIEnabled(false);
                    currentUpgrade = Mathf.Min((int)playerLevel / 5 - 1, 2);
                    player.GetComponent<PlayerController>().LoadWeapon(player.GetComponent<PlayerController>().weaponSlots[0], weaponUpgradeBranch[currentUpgrade]);

                    upgradeSelectAction.started -= func;
                    upgradeLocked = true;
                };

                upgradeSelectAction.started += func;
            }
            else if (playerLevel % 5f == 0f && upgradeLocked)
            {
                currentUpgrade = Mathf.Min((int)playerLevel / 5 - 1, 2);
                player.GetComponent<PlayerController>().LoadWeapon(player.GetComponent<PlayerController>().weaponSlots[0], weaponUpgradeBranch[currentUpgrade]);
            }
        };
    }

    public bool CanUpgrade()
    {
        return statPoints > 0;
    }
    public void DecrementStatPoints()
    {
        statPoints--;
        UIManager.UpdateStatPoints(statPoints);
    }
    void OnUpgradeSelected(InputAction.CallbackContext callbackContext)
    {
        int selection = (int)upgradeSelectAction.ReadValue<float>();
        switch (selection)
        {
            case 1:
                weaponUpgradeBranch = gunBranch;
                break;
            case 2:
                weaponUpgradeBranch = bigBranch;
                break;
            case 3:
                weaponUpgradeBranch = railBranch;
                break;
            case 4:
                weaponUpgradeBranch = flameBranch;
                break;
        }

        UIManager.SetUpgradeUIEnabled(false);
        currentUpgrade = (int)playerLevel / 5 - 1;
        player.GetComponent<PlayerController>().LoadWeapon(player.GetComponent<PlayerController>().weaponSlots[0], weaponUpgradeBranch[currentUpgrade]);
    }
    public float GetPlayerLevelUpRequirement(float level)
    {
        return math.pow((level + 1f) * experienceLinearRate, 1.0f + level * experienceExponentRate);
    }

    public void AddExperience(float experience)
    {
        playerExperience += experience;

        float levelUpRequirement = GetPlayerLevelUpRequirement(playerLevel);

        while (playerExperience >= levelUpRequirement)
        {

            playerExperience -= levelUpRequirement;
            playerLevel += 1.0f;
            levelUpRequirement = GetPlayerLevelUpRequirement(playerLevel);
            onLevelUp?.Invoke();

            statPoints++;
            
        }

        UIManager.UpdateExperienceBar(playerExperience / levelUpRequirement, playerLevel);

        UIManager.UpdateStatPoints(statPoints);
    }
}
