using UnityEngine;
using TankGame.Events;

public class Projectile : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Vector3 velocity;
    private Vector3 acceleration = Vector3.zero;

    private float lifetime = 0.0f;
    public float maxLifetime = 10.0f;
    
    public RaycastHitEvent onHit;
    
    private bool isHit = false;
    private LayerMask ignore;

    public int penetration = 1;


    
    public void Shoot(
        Vector3 at,
        Vector3 to,
        float speed, 
        float lifetime, 
        float spreadStrength, 
        LayerMask layerMask, 
        int pen
        )
    {
        Vector2 spread = UnityEngine.Random.insideUnitCircle * spreadStrength;
        Vector3 baseDirection = (to - at).normalized;

        Vector3 direction = baseDirection + Vector3.Cross(baseDirection, Vector3.up) * spread.x + Vector3.up * spread.y;
        velocity = direction * speed;
        ignore = layerMask;
        penetration = pen;

        maxLifetime = lifetime;

        transform.position = at;
        transform.LookAt(to);
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

            Destroy(gameObject, 5f);
            return;
        }
        Vector3 lastPosition = transform.position;

        velocity += acceleration * Time.deltaTime;
        transform.position += velocity * Time.deltaTime;

        Vector3 difference = transform.position - lastPosition;
        RaycastHit[] hits = Physics.RaycastAll(lastPosition, difference.normalized, difference.magnitude, ~ignore);
        
        UnityEngine.Debug.DrawRay(lastPosition, difference);

        if (hits.Length <= 0) {return;}
        System.Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance));

        int pierceCount = 0;
       
        
        foreach (RaycastHit hit in hits)
        {   
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Border"))
            {
                continue;
            }

            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Map"))
            {
                isHit = true;
                Destroy(gameObject, 5f);
                break;
            }

            if (pierceCount >= penetration)
            {
                isHit = true;
                Destroy(gameObject, 5f);
                break;
            }

            
            pierceCount++;
            onHit?.Invoke(hit);

            transform.position = hit.point;
        }
        
    }
}
