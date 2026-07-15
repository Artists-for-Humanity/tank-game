using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatUIManager : MonoBehaviour
{
    private static CombatUIManager _instance;
    public static CombatUIManager Instance {get {return _instance;}}

    public GameObject combatUI;
    public GameObject healthBar;
    public GameObject experienceBar;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else
        {
            _instance = this;
        }
    }

    public void UpdateHealthBar(float percentage)
    {
        healthBar.GetComponent<Slider>().value = percentage;
    }

    public void UpdateExperienceBar(float percentage, float level)
    {
        experienceBar.GetComponent<Slider>().value = percentage;
        experienceBar.transform.Find("LevelText").GetComponent<TextMeshProUGUI>().text = level.ToString();

    }



}
