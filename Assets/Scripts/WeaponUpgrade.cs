using UnityEngine;

[CreateAssetMenu(fileName = "WeaponUpgrade", menuName = "Scriptable Objects/WeaponUpgrade")]
public class WeaponUpgrade : ScriptableObject
{
    
    public float firerate;
    public float bulletDamage;
    public float bulletSpeed;
    public float bulletSpread;
    public float bulletLifetime;
    public int bulletsPerShot;

    public Mesh weaponMesh;
    public Material weaponMaterial;
    

    public Vector3 positionOffset = Vector3.zero;
    public Vector3 rotationOffset = Vector3.zero;
    public Vector3 scale = Vector3.one;

    public Vector3[] shootPositionOffsets;

}
