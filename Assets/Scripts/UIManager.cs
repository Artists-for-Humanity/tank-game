using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public static class UIManager
{
    public static bool upgradeUIEnabled = true;
    public static void SetUpgradeUIEnabled(bool enabled)
    {
        upgradeUIEnabled = enabled;
        GameObject upgradeUI = GameObject.Find("UpgradeUI");
        GameObject sBackground = upgradeUI.transform.Find("StatBackground").gameObject;
       
        if (enabled)
        {
            sBackground.transform.LeanMoveX(10f, 0.25f);
        } else
        {
            sBackground.transform.LeanMoveX(-500f, 0.25f);
        }
    }
    public static void ToggleUpgradeUI()
    {
        upgradeUIEnabled = !upgradeUIEnabled;

        SetUpgradeUIEnabled(upgradeUIEnabled);
    }

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

    public static void UpdateReloadBar(float percentage)
    {
        GameObject combatUI = GameObject.Find("CombatUI");
        GameObject crosshair = combatUI.transform.Find("Crosshair").gameObject;

        GameObject reloadBar = crosshair.transform.Find("ReloadBar").gameObject;
        reloadBar.GetComponent<Slider>().value = 1 - percentage;
    }

    public static void SetGameOverUIEnabled(bool enabled)
    {
        GameObject gameOverUI = GameObject.Find("GameOverUI");
        gameOverUI.GetComponent<Canvas>().enabled = enabled;
    }

}
