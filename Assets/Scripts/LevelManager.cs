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

        healthStatBar.onValueChanged = RefreshHealthStat;
        bulletDamageStatBar.onValueChanged = RefreshBulletDamageStat;
        bulletSpeedStatBar.onValueChanged = RefreshBulletSpeedStat;
        firerateStatBar.onValueChanged = RefreshFirerateStat;
        vehicleSpeedStatBar.onValueChanged = RefreshVehicleSpeedStat;

        SetVehicleUpgrade(vehicleUpgrade);
        SetWeaponUpgrade(weaponUpgrade);

        RefreshStats();


        onLevelUp += () =>
        {
            if (playerLevel % 5f == 0f)
            {
                weaponUpgradeBranch = gunBranch;
                currentUpgrade = (int)playerLevel / 5 - 1;

                SetWeaponUpgrade(weaponUpgradeBranch[currentUpgrade]);
            }
        };
    }

    public void SetVehicleUpgrade(VehicleUpgrade newVehicleUpgrade)
    {
        vehicleUpgrade = newVehicleUpgrade;

        GameObject vehicleMesh = player.transform.Find("VehicleMesh").gameObject;
        vehicleMesh.GetComponent<MeshFilter>().mesh = newVehicleUpgrade.vehicleMesh;
        vehicleMesh.GetComponent<MeshRenderer>().material = newVehicleUpgrade.vehicleMaterial;

        vehicleMesh.transform.localPosition = newVehicleUpgrade.positionOffset;
        vehicleMesh.transform.localEulerAngles = newVehicleUpgrade.rotationOffset;
        vehicleMesh.transform.localScale = newVehicleUpgrade.scale;

        player.GetComponent<Suspension>().mu = newVehicleUpgrade.sidewaysFriction;

        RefreshStats();
    }

    public void SetWeaponUpgrade(WeaponUpgrade newWeaponUpgrade)
    {
        weaponUpgrade = newWeaponUpgrade;

        GameObject weaponMesh = player.transform.Find("WeaponAxis").Find("WeaponMesh").gameObject;
        weaponMesh.GetComponent<MeshFilter>().mesh = newWeaponUpgrade.weaponMesh;
        weaponMesh.GetComponent<MeshRenderer>().material = newWeaponUpgrade.weaponMaterial;
        weaponMesh.transform.localPosition = newWeaponUpgrade.positionOffset;
        weaponMesh.transform.localEulerAngles = newWeaponUpgrade.rotationOffset;
        weaponMesh.transform.localScale = newWeaponUpgrade.scale;

        player.GetComponent<PlayerController>().bulletSpread = weaponUpgrade.bulletSpread;
        player.GetComponent<PlayerController>().bulletLifetime = weaponUpgrade.bulletLifetime;
        player.GetComponent<PlayerController>().bulletsPerShot = weaponUpgrade.bulletsPerShot;

        RefreshStats();
    }
    public void RefreshHealthStat(float oldValue, float newValue)
    {
        float healthStat = healthStatBar.value;

        player.GetComponent<HealthComponent>().maxHealth = vehicleUpgrade.health + healthStat * vehicleUpgrade.health / 2f;

        UIManager.UpdateHealthBar(player.GetComponent<HealthComponent>().HealthAsPercentage());
    }

    public void RefreshBulletDamageStat(float oldValue, float newValue)
    {
        float bulletDamageStat = bulletDamageStatBar.value;

        player.GetComponent<PlayerController>().bulletDamage = weaponUpgrade.bulletDamage + bulletDamageStat * 20f;
    }

    public void RefreshBulletSpeedStat(float oldValue, float newValue)
    {
        float bulletSpeedStat = bulletSpeedStatBar.value;

        player.GetComponent<PlayerController>().bulletSpeed = weaponUpgrade.bulletSpeed + bulletSpeedStat * 200f;
    }

    public void RefreshFirerateStat(float oldValue, float newValue)
    {
        float firerateStat = firerateStatBar.value;

        player.GetComponent<PlayerController>().firerate = weaponUpgrade.firerate - firerateStat / 20f * weaponUpgrade.firerate;
    }

    public void RefreshVehicleSpeedStat(float oldValue, float newValue)
    {
        float vehicleSpeedStat = vehicleSpeedStatBar.value;

        player.GetComponent<PlayerController>().vehicleSpeed = vehicleUpgrade.speed + vehicleSpeedStat / 10f * vehicleUpgrade.speed;
    }

    public void RefreshStats()
    {
        RefreshHealthStat(0f, 0f);
        RefreshBulletDamageStat(0f, 0f);
        RefreshBulletSpeedStat(0f, 0f);
        RefreshFirerateStat(0f, 0f);
        RefreshVehicleSpeedStat(0f, 0f);
    }

    public float GetPlayerLevelUpRequirement(float level)
    {
        return math.pow((level + 1f) * experienceLinearRate, 1.0f + level * experienceExponentRate);
    }

    public void AddExperience(float experience)
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

        UIManager.UpdateExperienceBar(playerExperience / levelUpRequirement, playerLevel);
    }
}
