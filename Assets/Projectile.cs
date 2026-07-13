using UnityEngine;
using UnityEngine.SocialPlatforms;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Rendering.Analytics;
using System;

public class Projectile : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Vector3 velocity;
    private float lifetime = 0.0f;
    public float maxLifetime = 10.0f;
    private Vector3 acceleration = Physics.gravity * 9.81f;
    public delegate void OnHit(RaycastHit? hit);
    public OnHit onHit;
    
    private bool isHit = false;
    

    public void Shoot(Vector3 initialVelocity, float lifetime)
    {
        velocity = initialVelocity;
        maxLifetime = lifetime;
    }

    // Update is called once per frame
    void Update()
    {
        if (isHit) {return;}
        lifetime += Time.deltaTime;

        Vector3 lastPosition = transform.position;

        velocity += acceleration * Time.deltaTime;
        transform.position += velocity * Time.deltaTime;

        RaycastHit hit;
        Vector3 difference = transform.position - lastPosition;

        Debug.DrawRay(lastPosition, difference, Color.green, 1.0f);
        if (Physics.Raycast(lastPosition, difference.normalized, out hit, difference.magnitude))
        {
            isHit = true;
        }

        if (lifetime >= maxLifetime)
        {
            isHit = true;
        }

        if (isHit)
        {
            onHit?.Invoke(hit);
        }
        
    }
}
