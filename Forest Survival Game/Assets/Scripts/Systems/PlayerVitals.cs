using UnityEngine;

public class PlayerVitals : MonoBehaviour
{
    [Header("Refrences")]
    public GameClock clock;
    public WaterCheck waterChecker;
    public PlayerHealth health;

    [Header("Hunger")]
    [Range(0, 100)] public float hunger = 50f;
    public float hungerDrainPerMinute = 0.5f;

    [Header("Thirst")]
    [Range(0, 100)] public float thirst = 50f;
    public float thirstDrainPerMinute = 0.8f;

    [Header("Temperature")]
    public float bodyTemp = 37f;
    public float tempChangeSpeed = 0.25f;
    public float coldThreshold = 35f;
    public float hotThreshold = 39f;

    [Header("Wetness")]
    [Range(0, 100)] public float wetness = 0f;
    public float dryRatePerMinute = 2f;

    float GetAmbientTemp()
    {
        float h = clock.timeOfDay;

        float dayTemp = 24f;
        float nightTemp = 10f;

        float t = Mathf.Abs(h - 12f) / 12f;
        float blend = 1f - t;

        return Mathf.Lerp(nightTemp, dayTemp, blend);
    }

    void Update()
    {
        float dt = Time.deltaTime;

        //Hunger Drain
        hunger -= hungerDrainPerMinute * (dt / 60f);
        hunger = Mathf.Clamp(hunger, 0f, 100f);

        // Thirst drain
        thirst -= thirstDrainPerMinute * (dt / 60f);
        thirst = Mathf.Clamp(thirst, 0f, 100f);
        if (thirst <= 0f)
        {
            //add more penalties here
            //bodyTemp -= 0.1f * dt;  // dries out, gets colder
        }

        //Wet Mechanics
        float ambient = GetAmbientTemp();
        float warmthBonus = Mathf.InverseLerp(5, 25f, ambient);
        float dry = dryRatePerMinute * (1 + warmthBonus) * (dt / 60f);
        wetness -= dry;
        wetness = Mathf.Clamp(wetness, 0f, 100f);

        //Temp Control
        float targetBody = 37f + (ambient - 15f) * 0.05f;
        float wetColdFactor = 1f + (wetness / 100f) * 1.5f;
        bodyTemp = Mathf.Lerp(bodyTemp, targetBody, tempChangeSpeed * wetColdFactor * dt);

        if (waterChecker.isInWater)
        {
            thirst = Mathf.MoveTowards(thirst, 100, 25f * dt);
        }

        StarveThirstPunish();
        Heal();
    }

    public string GetTempState()
    {
        if (bodyTemp <= coldThreshold) return "Cold";
        if (bodyTemp >= hotThreshold) return "Hot";
        return "Ok";
    }

    public float AmbientTempForDebug() => GetAmbientTemp();


    public void StarveThirstPunish()
    {
        if (thirst <= 0 || hunger <= 0)
        {
            DrainHp();
        }
    }

    public void Heal()
    {
        float currentHP = health.getHealth();
        if (currentHP == 100) return;

        if (thirst > 10 && hunger > 10)
        {
            health.SetHealth(currentHP += 1f * (Time.deltaTime / 60f));
        }
    }


    void DrainHp()
    {
        float currentHP = health.getHealth();
        if (currentHP <= 0) return;

        health.SetHealth(currentHP -= 2f * (Time.deltaTime / 60f));
    }


}
