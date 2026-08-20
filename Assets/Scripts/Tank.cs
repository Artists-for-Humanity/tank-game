
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TankStats
{
    public float health;
    public float vehicleSpeed;
}

[Serializable]
public class WeaponStats
{
    public float bulletDamage;
    public float bulletSpeed;
    public float firerate;
    public float bulletLifetime;
    public float bulletSpread;
    public int bulletsPerShot;
}

[Serializable]
public class WeaponSlot
{
    public GameObject weaponAxis;
    public GameObject shootPosition;
    public WeaponUpgrade weaponUpgrade;
}

public class Tank : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public Rigidbody rigidBody;


    public HealthComponent healthComponent;

    public float baseRotationSpeed = 5;
    public float turretRotationSpeed = 5;

    protected float attackTimer = 0.0f;
    

    public VehicleUpgrade currentVehicle;

    public WeaponSlot[] weaponSlots;

    public TankStats statMultipliers = new TankStats()
    {
        health = 1f,
        vehicleSpeed = 1f
    };

    public WeaponStats weaponStatMultipiers = new WeaponStats()
    {
        bulletDamage = 1f,
        firerate = 1.0f,
        bulletSpeed = 1f,
        bulletLifetime = 1f,
        bulletSpread = 1f,
        bulletsPerShot = 1,
    };


    public TankStats tankStats = new TankStats()
    {
        health = 100,
        vehicleSpeed = 100f,
    };

    protected virtual void Start()
    {
        
    }

    protected void Move(Vector3 direction)
    {
        rigidBody.linearVelocity += direction * Time.fixedDeltaTime * tankStats.vehicleSpeed;
    }

    protected void RotateBase(Vector3 direction)
    {
        direction.y = 0;
        direction.Normalize();

        //Quaternion baseRotationTarget = Quaternion.LookRotation(direction);
        //transform.rotation = Quaternion.Slerp(transform.rotation, baseRotationTarget, Time.deltaTime * baseRotationSpeed);
        
        
        Quaternion diff = Quaternion.LookRotation(direction) * Quaternion.Inverse(transform.rotation);

        diff.ToAngleAxis(out float angle, out Vector3 axis);

        if (angle > 180f)
        {
            angle -= 360f;
        }

        float angleRad = angle * Mathf.Deg2Rad;

        Vector3 goalAngularVelocity = axis.normalized * angleRad * baseRotationSpeed;
        rigidBody.angularVelocity = Vector3.Lerp(rigidBody.angularVelocity, goalAngularVelocity, Time.fixedDeltaTime * baseRotationSpeed);
    }
    protected void PointGun(Vector3 direction)
    {
        direction.y = 0;
        direction.Normalize();

        Quaternion rotationTarget = Quaternion.LookRotation(direction);
        foreach (WeaponSlot weaponSlot in weaponSlots)
        {
            weaponSlot.weaponAxis.transform.rotation = Quaternion.Slerp(weaponSlot.weaponAxis.transform.rotation, rotationTarget, Time.fixedDeltaTime * turretRotationSpeed);
            weaponSlot.weaponAxis.transform.localEulerAngles = new Vector3(0, weaponSlot.weaponAxis.transform.localEulerAngles.y, 0);
        }
    }

    #nullable enable
    protected void ShootGun(Vector3 to, LayerMask layerMask, Action<RaycastHit>? onHit = null)
    {
        
        foreach (WeaponSlot weaponSlot in weaponSlots)
        {
            AudioSource audioSource = AudioUtils.PlayClipAt(weaponSlot.weaponUpgrade.fireSoundClip, transform.position);
            audioSource.volume = 0.5f;
            audioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
            audioSource.minDistance = 10f;
            audioSource.maxDistance = 100f;

            audioSource.rolloffMode = AudioRolloffMode.Linear;
            audioSource.spatialBlend = 1;

            for (int i = 0; i < weaponSlot.weaponUpgrade.bulletsPerShot; i++)
            {
                GameObject bullet = Instantiate(weaponSlot.weaponUpgrade.projectile);

                Projectile projectileScript = bullet.GetComponent<Projectile>();
                
                projectileScript.Shoot(
                    weaponSlot.shootPosition.transform.position, 
                    to, 
                    weaponSlot.weaponUpgrade.bulletSpeed        * weaponStatMultipiers.bulletSpeed, 
                    weaponSlot.weaponUpgrade.bulletLifetime     * weaponStatMultipiers.bulletLifetime, 
                    weaponSlot.weaponUpgrade.bulletSpread       * weaponStatMultipiers.bulletSpread,  
                    layerMask, 
                    weaponSlot.weaponUpgrade.penetration
                    );

            HashSet<HealthComponent> alreadyHit = new HashSet<HealthComponent>();

    
                projectileScript.onHit += (RaycastHit hit) =>
                {
                    if (hit.transform.gameObject != null)
                    {
                        HealthComponent enemyHealthComponent = hit.transform.gameObject.GetComponent<HealthComponent>();
                        if (enemyHealthComponent != null)
                        {
                            if (!alreadyHit.Add(enemyHealthComponent))
                            {
                                return;
                            }
                            enemyHealthComponent?.TakeDamage(weaponSlot.weaponUpgrade.bulletDamage * weaponStatMultipiers.bulletDamage);
                        }

                        if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Map"))
                        {
                            AudioSource impactAudioSource = AudioUtils.PlayClipAt(weaponSlot.weaponUpgrade.impactSoundClip, hit.point);
                            impactAudioSource.volume = 0.2f;
                            impactAudioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
                            impactAudioSource.minDistance = 0f;
                            impactAudioSource.maxDistance = 75f;

                            impactAudioSource.rolloffMode = AudioRolloffMode.Linear;
                            impactAudioSource.spatialBlend = 1;
                        }
                        
                        if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                        {
                            AudioSource hitmarkerAudioSource = AudioUtils.PlayClipAt(GameManager.Instance.hitmarker, hit.point);
                            hitmarkerAudioSource.volume = 0.5f;
                            hitmarkerAudioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);

                            GameManager.Instance.hitmarkerTimer = 0.1f;
                        }


                    }
                };
            }
        }
    }



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

    
    #nullable enable
    public void LoadWeapon(WeaponSlot weaponSlot, WeaponUpgrade? weapon)
    {
        if (weapon != null)
        {
            weaponSlot.weaponUpgrade = weapon;
        }
        
        GameObject weaponMesh = weaponSlot.weaponAxis.transform.Find("WeaponMesh").gameObject;
        weaponMesh.GetComponent<MeshFilter>().mesh = weaponSlot.weaponUpgrade.weaponMesh;
        weaponMesh.GetComponent<MeshRenderer>().material = weaponSlot.weaponUpgrade.weaponMaterial;
        weaponMesh.transform.localPosition = weaponSlot.weaponUpgrade.positionOffset;
        weaponMesh.transform.localEulerAngles = weaponSlot.weaponUpgrade.rotationOffset;
        weaponMesh.transform.localScale = weaponSlot.weaponUpgrade.scale;

        RefreshStats();
    }

    public void RefreshStats()
    {
        healthComponent.maxHealth = currentVehicle.health * statMultipliers.health;
        tankStats.vehicleSpeed = currentVehicle.speed * statMultipliers.vehicleSpeed;
    }
}
