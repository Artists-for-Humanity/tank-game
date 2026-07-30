using UnityEngine;




public class UpgradeableTank : Tank
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public VehicleUpgrade currentVehicle;
    public WeaponUpgrade currentWeapon;

    public TankStats statMultipliers = new TankStats()
    {
        health = 1f,
        bulletDamage = 1f,
        bulletSpeed = 1f,
        firerate = 1f,
        vehicleSpeed = 1f
    };


    public void LoadVehicle(VehicleUpgrade vehicle)
    {
        currentVehicle = vehicle;

        GameObject vehicleMesh = transform.Find("VehicleMesh").gameObject;
        vehicleMesh.GetComponent<MeshFilter>().mesh = currentVehicle.vehicleMesh;
        vehicleMesh.GetComponent<MeshRenderer>().material = currentVehicle.vehicleMaterial;

        vehicleMesh.transform.localPosition = currentVehicle.positionOffset;
        vehicleMesh.transform.localEulerAngles = currentVehicle.rotationOffset;
        vehicleMesh.transform.localScale = currentVehicle.scale;

        GetComponent<Suspension>().mu = currentVehicle.sidewaysFriction;

        RefreshStats();
    }

    public void LoadWeapon(WeaponUpgrade weapon)
    {
        currentWeapon = weapon;

        GameObject weaponMesh = transform.Find("WeaponAxis").Find("WeaponMesh").gameObject;
        weaponMesh.GetComponent<MeshFilter>().mesh = currentWeapon.weaponMesh;
        weaponMesh.GetComponent<MeshRenderer>().material = currentWeapon.weaponMaterial;
        weaponMesh.transform.localPosition = currentWeapon.positionOffset;
        weaponMesh.transform.localEulerAngles = currentWeapon.rotationOffset;
        weaponMesh.transform.localScale = currentWeapon.scale;

        RefreshStats();
    }

    public void RefreshStats()
    {
        healthComponent.maxHealth = currentVehicle.health * statMultipliers.health;

        tankStats.bulletDamage = currentWeapon.bulletDamage * statMultipliers.bulletDamage;
        tankStats.bulletSpeed = currentWeapon.bulletSpeed * statMultipliers.bulletSpeed;
        tankStats.firerate = currentWeapon.firerate * statMultipliers.firerate;
        tankStats.vehicleSpeed = currentVehicle.speed * statMultipliers.vehicleSpeed;
        
        tankStats.bulletLifetime = currentWeapon.bulletLifetime;
        tankStats.bulletSpread = currentWeapon.bulletSpread;
        tankStats.bulletsPerShot = currentWeapon.bulletsPerShot;
    }
}
