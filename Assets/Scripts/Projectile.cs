using UnityEngine;
using UnityEngine.SocialPlatforms;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Rendering.Analytics;
using System;
using TankGame.Events;

public class Projectile : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Vector3 velocity;
    private Vector3 acceleration = Physics.gravity * 9.81f;

    private float lifetime = 0.0f;
    public float maxLifetime = 10.0f;
    
    public RaycastHitEvent onHit;
    
    private bool isHit = false;
    private LayerMask ignore;

    public int penetration = 10;


    public void Shoot(Vector3 initialVelocity, float lifetime, LayerMask layerMask, int pen)
    {
        velocity = initialVelocity;
        maxLifetime = lifetime;
        ignore = layerMask;
        penetration = pen;
        
        transform.LookAt(transform.position + velocity);
    }
    public void ShootWithSpread(Vector3 initialVelocity, float lifetime, float spreadStrength, LayerMask layerMask, int pen)
    {
        Vector2 spread = UnityEngine.Random.insideUnitCircle * spreadStrength;

        Vector3 direction = initialVelocity.normalized + Vector3.Cross(initialVelocity.normalized, Vector3.up) * spread.x + Vector3.up * spread.y;
        velocity = direction * initialVelocity.magnitude;
        ignore = layerMask;
        penetration = pen;

        transform.LookAt(transform.position + velocity);
    }

    

    // Update is called once per frame
    void Update()
    {
        if (isHit) {return;}

        transform.LookAt(transform.position + velocity);
        lifetime += Time.deltaTime;
        if (lifetime >= maxLifetime)
        {
            isHit = true;

            Destroy(gameObject);
            return;
        }
        Vector3 lastPosition = transform.position;

        velocity += acceleration * Time.deltaTime;
        transform.position += velocity * Time.deltaTime;

        Vector3 difference = transform.position - lastPosition;
        RaycastHit[] hits = Physics.RaycastAll(lastPosition, difference.normalized, difference.magnitude, ~ignore);
        
        Debug.DrawRay(lastPosition, difference);

        if (hits.Length <= 0) {return;}
        System.Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance));

        int pierceCount = 0;

        foreach (RaycastHit hit in hits)
        {
            if (pierceCount >= penetration)
            {
                isHit = true;
                Destroy(gameObject);
                return;
            }

            pierceCount++;
            onHit?.Invoke(hit);
        }
        
    }
}
