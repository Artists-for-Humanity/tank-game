
using UnityEngine;

public class WarshipAI : EnemyAI
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void UpdateFollow()
    {
        foreach (Transform child in GameObject.Find("SeaPositions").transform)
        {
            follow = child.gameObject;
        }
    }
}
