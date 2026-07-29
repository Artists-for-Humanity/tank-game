using UnityEngine;

[CreateAssetMenu(fileName = "VehicleUpgrade", menuName = "Scriptable Objects/VehicleUpgrade")]
public class VehicleUpgrade : ScriptableObject
{
    public float sidewaysFriction;
    public float speed;
    public float health;

    public Mesh vehicleMesh;
    public Material vehicleMaterial;
    
    public Vector3 positionOffset = Vector3.zero;
    public Vector3 rotationOffset = Vector3.zero;
    public Vector3 scale = Vector3.one;
}
