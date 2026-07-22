using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class CombatUIManager
{
    
    public static void UpdateHealthBar(float percentage)
    {
        GameObject combatUI = GameObject.Find("CombatUI");
        GameObject healthBar = combatUI.transform.Find("HealthBar").gameObject;
       
        healthBar.GetComponent<Slider>().value = percentage;
    }

    public static void UpdateExperienceBar(float percentage, float level)
    {
        GameObject combatUI = GameObject.Find("CombatUI");
        GameObject experienceBar = combatUI.transform.Find("ExperienceBar").gameObject;
        experienceBar.GetComponent<Slider>().value = percentage;
        experienceBar.transform.Find("LevelText").GetComponent<TextMeshProUGUI>().text = level.ToString();

    }



}
