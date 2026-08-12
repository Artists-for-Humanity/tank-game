using UnityEngine;
using UnityEngine.UI;
using TankGame.Events;
using Unity.Mathematics;
using Unity.VisualScripting;
public class StatBar : MonoBehaviour
{
    private Button upgradeButton;
    private Slider bar;
    public GameObject segment;
    public ValueChangedEvent<float> onValueChanged;
    public float value;
    public float maxValue = 10.0f;
    public float percentage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        upgradeButton = transform.Find("UpgradeButton").GetComponent<Button>();
        bar = GetComponent<Slider>();

        upgradeButton.onClick.AddListener(OnButtonClicked);
       
        int segmentCount = (int)maxValue;
        for (int i = 1; i < segmentCount; i++)
        {
            GameObject newSegment = Instantiate(segment, transform);
            newSegment.GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, 0f);

            newSegment.GetComponent<RectTransform>().anchorMin = new Vector2((float)i/maxValue, 0.5f);
            newSegment.GetComponent<RectTransform>().anchorMax = new Vector2((float)i/maxValue, 0.5f);
        }
    }


    void OnButtonClicked()
    {
        if (!LevelManager.Instance.CanUpgrade())
        {
            return;
        }
        LevelManager.Instance.DecrementStatPoints();
        
        float oldValue = value;
        float newValue = math.clamp(value + 1.0f, 0f, maxValue);

        value = newValue;
        percentage = value/maxValue;

        bar.value = percentage; 

        onValueChanged?.Invoke(oldValue, newValue);
    }
    
}
