using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class UIManager
{
    public static bool upgradeUIEnabled = true;
    public static void UpdateWaveUI(int wave)
    {
         GameObject combatUI = GameObject.Find("CombatUI");
        GameObject waveText = combatUI.transform.Find("WaveText").gameObject;

        waveText.GetComponent<TextMeshProUGUI>().text = "You are on wave: " + wave.ToString() + "!";
    }

    public static void UpdateStatPoints(int amount)
    {
        GameObject upgradeUI = GameObject.Find("UpgradeUI");
        GameObject sBackground = upgradeUI.transform.Find("StatBackground").gameObject;
        GameObject statpoints = sBackground.transform.Find("StatPoints").gameObject;
       statpoints.GetComponent<TextMeshProUGUI>().text = "Stat Points: " + amount.ToString();
    }
    public static void SetUpgradeUIEnabled(bool enabled)
    {
        GameObject upgradeUI = GameObject.Find("UpgradeUI");
        GameObject uContainer = upgradeUI.transform.Find("UpgradeContainer").gameObject;
       
        if (enabled)
        {
            uContainer.GetComponent<RectTransform>().LeanMoveY(-80f, 0.25f);
        } else
        {
            uContainer.GetComponent<RectTransform>().LeanMoveY(1000f, 0.25f);
        }
    }

    public static void SetVehicleUpgradeUIEnabled(bool enabled)
    {
        GameObject upgradeUI = GameObject.Find("UpgradeUI");
        GameObject uContainer = upgradeUI.transform.Find("VehicleUpgradeContainer").gameObject;
       
        if (enabled)
        {
            uContainer.GetComponent<RectTransform>().LeanMoveY(-80f, 0.25f);
        } else
        {
            uContainer.GetComponent<RectTransform>().LeanMoveY(1000f, 0.25f);
        }
    }

    public static void SetStatUIEnabled(bool enabled)
    {
        upgradeUIEnabled = enabled;
        GameObject upgradeUI = GameObject.Find("UpgradeUI");
        GameObject sBackground = upgradeUI.transform.Find("StatBackground").gameObject;
       
        if (enabled)
        {
            sBackground.transform.LeanMoveX(10f, 0.25f);
        } else
        {
            sBackground.transform.LeanMoveX(-1000f, 0.25f);
        }
    }
    public static void ToggleStatUI()
    {
        upgradeUIEnabled = !upgradeUIEnabled;
        if (upgradeUIEnabled)
        {
            UnityEngine.Cursor.lockState = CursorLockMode.None;
        } else
        {
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        }
        

        SetStatUIEnabled(upgradeUIEnabled);
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
        Slider reloadSlider = reloadBar.GetComponent<Slider>();
        reloadSlider.value = 1f - percentage;
    }

    public static void UpdateHitmarker()
    {
        GameObject combatUI = GameObject.Find("CombatUI");
        GameObject crosshair = combatUI.transform.Find("Crosshair").gameObject;
        GameObject hitmarker = crosshair.transform.Find("Hitmarker").gameObject;

        
    }

    public static void SetGameOverUIEnabled(bool enabled)
    {
        GameObject gameOverUI = GameObject.Find("GameOverUI");
        gameOverUI.GetComponent<Canvas>().enabled = enabled;
    }

}
