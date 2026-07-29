using System;
using TankGame.Events;
using Unity.Mathematics;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    public float health;
    public float maxHealth;

    public bool hasRegen = true;
    public float regenPerTick = 1f;
    private float regenTickTimer = 0f;
    public float regenTickInterval = 0.1f;
    private float regenDelayTimer = 0f;
    public float regenDelayInterval = 2f;

    public ValueChangedEvent<float> healthChanged;
    public Action onDied;
    public bool isDead = false;

    public float HealthAsPercentage()
    {
        return health/maxHealth;
    }
    public void Heal(float healAmount)
    {
        float oldHealth = health;
        float newHealth = health + healAmount;
        health = math.clamp(newHealth, 0f, maxHealth);

        healthChanged?.Invoke(oldHealth, newHealth);
    }
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

        regenDelayTimer = 0f;
    }

    void Update()
    {
        if (!hasRegen) {return;}

        regenDelayTimer += Time.deltaTime;
        if (regenDelayTimer > regenDelayInterval)
        {
            regenTickTimer += Time.deltaTime;
            if (regenTickTimer > regenTickInterval)
            {
                Heal(regenPerTick);
                regenTickTimer = 0f;
            }
        }
    }
}
