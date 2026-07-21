using UnityEngine;
using Unity.Mathematics;
using UnityEngine.InputSystem.Interactions;
using TankGame.Events;

public static class LevelManager
{
    public static float playerLevel = 0.0f;
    public static float playerExperience = 0.0f;

    private const float experienceExponentRate = 0.005f;
    private const float experienceLinearRate = 100f;

    private static GameObject UpgradeUI;
    private static StatBar healthStatBar;
    private static StatBar bulletDamageStatBar;
    private static StatBar bulletSpeedStatBar;
    private static StatBar firerateStatBar;
    private static StatBar vehicleSpeedStatBar;

    private static GameObject player;
    public static ParameterlessEvent onLevelUp;

    public static WeaponUpgrade weaponUpgrade;
    public static VehicleUpgrade vehicleUpgrade;

    public static void Initialize()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        UpgradeUI = GameObject.Find("UpgradeUI");
        Transform panel = UpgradeUI.transform.Find("Panel");

        healthStatBar = panel.Find("StatBar5").gameObject.GetComponent<StatBar>();
        bulletDamageStatBar = panel.Find("StatBar4").gameObject.GetComponent<StatBar>();
        bulletSpeedStatBar = panel.Find("StatBar3").gameObject.GetComponent<StatBar>();
        firerateStatBar = panel.Find("StatBar2").gameObject.GetComponent<StatBar>();
        vehicleSpeedStatBar = panel.Find("StatBar1").gameObject.GetComponent<StatBar>();

        healthStatBar.onValueChanged = RefreshHealthStat;
        bulletDamageStatBar.onValueChanged = RefreshBulletDamageStat;
        bulletSpeedStatBar.onValueChanged = RefreshBulletSpeedStat;
        firerateStatBar.onValueChanged = RefreshFirerateStat;
        vehicleSpeedStatBar.onValueChanged = RefreshVehicleSpeedStat;
    }

    public static void SetVehicleUpgrade(VehicleUpgrade newVehicleUpgrade)
    {
        vehicleUpgrade = newVehicleUpgrade;

        GameObject vehicleMesh = player.transform.Find("VehicleMesh").gameObject;
        vehicleMesh.GetComponent<MeshFilter>().mesh = newVehicleUpgrade.vehicleMesh;
        RefreshStats();
    }

    public static void SetWeaponUpgrade(WeaponUpgrade newWeaponUpgrade)
    {
        weaponUpgrade = newWeaponUpgrade;

        GameObject weaponMesh = player.transform.Find("TurretAxis").Find("WeaponMesh").gameObject;
        weaponMesh.GetComponent<MeshFilter>().mesh = newWeaponUpgrade.weaponMesh;
        RefreshStats();
    }
    public static void RefreshHealthStat(float oldValue, float newValue)
    {
        float healthStat = healthStatBar.value;

        player.GetComponent<HealthComponent>().maxHealth = vehicleUpgrade.health + healthStat * 20f;

        CombatUIManager.UpdateHealthBar(player.GetComponent<HealthComponent>().HealthAsPercentage());
    }

    public static void RefreshBulletDamageStat(float oldValue, float newValue)
    {
        float bulletDamageStat = bulletDamageStatBar.value;

        player.GetComponent<PlayerController>().bulletDamage = weaponUpgrade.bulletDamage + bulletDamageStat * 20f;
    }

    public static void RefreshBulletSpeedStat(float oldValue, float newValue)
    {
        float bulletSpeedStat = bulletSpeedStatBar.value;

        player.GetComponent<PlayerController>().bulletSpeed = weaponUpgrade.bulletSpeed + bulletSpeedStat * 200f;
    }

    public static void RefreshFirerateStat(float oldValue, float newValue)
    {
        float firerateStat = firerateStatBar.value;

        player.GetComponent<PlayerController>().firerate = weaponUpgrade.firerate - firerateStat * 0.05f;
    }

    public static void RefreshVehicleSpeedStat(float oldValue, float newValue)
    {
        float vehicleSpeedStat = vehicleSpeedStatBar.value;

        player.GetComponent<PlayerController>().vehicleSpeed = vehicleUpgrade.speed + vehicleSpeedStat * 2f;
    }

    public static void RefreshStats()
    {
        RefreshHealthStat(0f, 0f);
        RefreshBulletDamageStat(0f, 0f);
        RefreshBulletSpeedStat(0f, 0f);
        RefreshFirerateStat(0f, 0f);
        RefreshVehicleSpeedStat(0f, 0f);
    }

    public static float GetPlayerLevelUpRequirement(float level)
    {
        return math.pow((level + 1f) * experienceLinearRate, 1.0f + level * experienceExponentRate);
    }

    public static void AddExperience(float experience)
    {
        playerExperience += experience;

        float levelUpRequirement = GetPlayerLevelUpRequirement(playerLevel);

        Debug.Log(playerLevel);
        Debug.Log(playerExperience);
        Debug.Log(levelUpRequirement.ToString());


        if (playerExperience >= levelUpRequirement)
        {
            playerExperience = 0.0f;
            playerLevel += 1.0f;

            onLevelUp?.Invoke();
        }

        CombatUIManager.UpdateExperienceBar(playerExperience / levelUpRequirement, playerLevel);
    }
}
