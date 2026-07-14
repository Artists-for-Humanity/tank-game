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
    private float lifetime = 0.0f;
    public float maxLifetime = 10.0f;
    private Vector3 acceleration = Physics.gravity * 9.81f;
    public RaycastHitEvent onHit;
    
    private bool isHit = false;
    

    public void Shoot(Vector3 initialVelocity, float lifetime)
    {
        velocity = initialVelocity;
        maxLifetime = lifetime;
    }
    public void ShootWithSpread(Vector3 initialVelocity, float lifetime, float spreadStrength)
    {
        Vector2 spread = UnityEngine.Random.insideUnitCircle * spreadStrength;

        Vector3 direction = initialVelocity.normalized + new Vector3(spread.x, spread.y, 0.0f);
        initialVelocity = direction * initialVelocity.magnitude;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isHit) {return;}
    
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

        RaycastHit hit;
        Vector3 difference = transform.position - lastPosition;

        Debug.DrawRay(lastPosition, difference, Color.green, 1.0f);
        if (Physics.Raycast(lastPosition, difference.normalized, out hit, difference.magnitude))
        {
            isHit = true;
            onHit?.Invoke(hit);

            Destroy(gameObject);
        }
        
    }
}
