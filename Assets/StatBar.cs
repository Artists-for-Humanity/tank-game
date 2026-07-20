using UnityEngine;
using UnityEngine.UI;
using TankGame.Events;
using Unity.Mathematics;
public class StatBar : MonoBehaviour
{

    
    private Button upgradeButton;
    private Slider bar;
    public ValueChangedEvent<float> onValueChanged;
    public float value;
    public float maxValue = 10.0f;
    public float percentage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        upgradeButton = transform.Find("UpgradeButton").GetComponent<Button>();
        bar = GetComponent<Slider>();

        upgradeButton.onClick.AddListener(OnButtonClicked);
    }


    void OnButtonClicked()
    {
        float oldValue = value;
        float newValue = math.clamp(value + 1.0f, 0f, maxValue);

        value = newValue;
        percentage = value/maxValue;

        bar.value = percentage; 

        onValueChanged?.Invoke(oldValue, newValue);
    }
    
}
