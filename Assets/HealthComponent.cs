using TankGame.Events;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    public float health;
    public float maxHealth;

    public ValueChangedEvent<float> healthChanged;
    public ParameterlessEvent onDied;
    public bool isDead = false;


    public void TakeDamage(float damage)
    {
        
        float oldHealth = health;
        float newHealth = health - damage;

        health = newHealth;

        healthChanged?.Invoke(oldHealth, newHealth);

        bool justDied = oldHealth > 0f && newHealth <= 0f;
        if (justDied)
        {
            isDead = true;
            onDied?.Invoke();
        }
    }

}
