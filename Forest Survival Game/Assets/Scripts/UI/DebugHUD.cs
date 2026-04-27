using TMPro;
using UnityEngine;

public class DebugHUD : MonoBehaviour
{
    [Header("References")]
    public GameClock clock;
    public PlayerVitals vitals;
    public PlayerHealth health;

    [Header("UI Elements")]
    public TextMeshProUGUI dayText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI hungerText;
    public TextMeshProUGUI thirstText;
    public TextMeshProUGUI ambientTempText;
    public TextMeshProUGUI bodyTempText;
    public TextMeshProUGUI wetnessText;
    public TextMeshProUGUI healthText;

    void Update()
    {
        if (!clock || !vitals) return;

        if (dayText)
            dayText.text = $"Day {clock.day}";

        if (timeText)
            timeText.text = $"Time {clock.GetTimeString()}  {(clock.IsNight ? "Night" : "Day")}";

        if (hungerText)
            hungerText.text = $"Hunger: {vitals.hunger:0}";

        if (thirstText)
            thirstText.text = $"Thirst: {vitals.thirst:0}";

        if (ambientTempText)
            ambientTempText.text = $"Ambient: {vitals.AmbientTempForDebug():0.0}°C";

        if (bodyTempText)
            bodyTempText.text = $"Body: {vitals.bodyTemp:0.0}°C ({vitals.GetTempState()})";

        if (wetnessText)
            wetnessText.text = $"Wetness: {vitals.wetness:0}%";

        if (healthText)
            healthText.text = $"Health: {health.getHealth():0}%";
    }
}