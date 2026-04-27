
using UnityEngine;

public class GameClock : MonoBehaviour
{
    [Header("Time")]
    [Range(0f, 24f)] public float timeOfDay = 8f;
    public int day = 1;

    [Tooltip("How many real seconds equal to 1 in game hour")]
    public float realSecondsPerGameHour = 60f;

    public float Time01 => timeOfDay / 24f;
    public bool IsNight => timeOfDay < 6f || timeOfDay > 18f;

    void Update()
    {
        float hoursPerSecond = 1f / realSecondsPerGameHour;
        timeOfDay += hoursPerSecond * Time.deltaTime;

        if (timeOfDay >= 24f)
        {
            timeOfDay -= 24f;
            day++;
        }

    }

    public string GetTimeString()
    {
        int hours = Mathf.FloorToInt(timeOfDay);
        int minutes = Mathf.FloorToInt((timeOfDay - hours) * 60f);

        return $"{hours:00}:{minutes:00}";
    }


}
