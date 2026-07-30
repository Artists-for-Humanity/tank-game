using UnityEngine;




public class UpgradeableTank : Tank
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

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
        
    }

    public void LoadWeapon(WeaponUpgrade weapon)
    {
        
    }

    public void refreshStats()
    {
        
    }
}
