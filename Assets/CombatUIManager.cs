using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class CombatUIManager
{
    public static GameObject combatUI;
    public static GameObject healthBar;
    public static GameObject experienceBar;

    public static void Initialize()
    {
        combatUI = GameObject.Find("CombatUI");
        healthBar = combatUI.transform.Find("HealthBar").gameObject;
        experienceBar = combatUI.transform.Find("ExperienceBar").gameObject;
    }

    public static void UpdateHealthBar(float percentage)
    {
        healthBar.GetComponent<Slider>().value = percentage;
    }

    public static void UpdateExperienceBar(float percentage, float level)
    {
        experienceBar.GetComponent<Slider>().value = percentage;
        experienceBar.transform.Find("LevelText").GetComponent<TextMeshProUGUI>().text = level.ToString();

    }



}
