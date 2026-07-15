using UnityEngine;
using Unity.Mathematics;
using UnityEngine.InputSystem.Interactions;
using TankGame.Events;

public static class LevelManager
{
    public static float playerLevel = 0.0f;
    public static float playerExperience = 0.0f;

    public static ParameterlessEvent onLevelUp;


    public static float GetPlayerLevelUpRequirement(float level)
    {
        return math.pow((level + 1f) * 100f, 1.0f + level * .01f);
    }

    public static void AddExperience(float experience)
    {
        playerExperience += experience;

        float levelUpRequirement = GetPlayerLevelUpRequirement(playerLevel);

        Debug.Log(playerLevel);
        Debug.Log(playerExperience);
        Debug.Log(levelUpRequirement.ToString());

        
        if (playerExperience >= levelUpRequirement)
        {
            playerExperience = 0.0f;
            playerLevel += 1.0f;

            onLevelUp?.Invoke();
        }

        CombatUIManager.Instance.UpdateExperienceBar(playerExperience/levelUpRequirement, playerLevel);

    }
}
