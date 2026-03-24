using TMPro;
using UnityEngine;

public class GameTime : MonoBehaviour
{
    [Header("Time Settings")]
    [SerializeField]
    private float dayLengthInMinutes = 10f;

    [SerializeField]
    private int startHour = 8;

    [Header("UI")]
    [SerializeField]
    private TextMeshProUGUI timeText;

    [SerializeField]
    private string timeFormat = "{0:00}:{1:00}"; 

    private float currentTimeOfDay = 0f; 
    private int currentHour;
    private int currentMinute;

    private void Start()
    {
        currentTimeOfDay = startHour / 24f;
        UpdateTimeValues();
        UpdateTimeDisplay();
    }

    private void Update()
    {
        currentTimeOfDay += Time.deltaTime / (dayLengthInMinutes * 80f);

        if (currentTimeOfDay >= 1f)
        {
            currentTimeOfDay -= 1f;
            OnNewDay(); 
        }

        UpdateTimeValues();
        UpdateTimeDisplay();
    }

    private void UpdateTimeValues()
    {
        float totalHours = currentTimeOfDay * 24f;
        currentHour = Mathf.FloorToInt(totalHours);
        currentMinute = Mathf.FloorToInt((totalHours - currentHour) * 60f);
    }

    private void UpdateTimeDisplay()
    {
        if (timeText != null)
        {
            timeText.text = string.Format(timeFormat, currentHour, currentMinute);
        }
    }

    private void OnNewDay()
    {
        Debug.Log("New day has begun!");
        // Логика нового дня (сброс каких-то значений, спавн новых предметов и т.д.)
    }

    public int GetCurrentHour()
    {
        return currentHour;
    }

    public int GetCurrentMinute()
    {
        return currentMinute;
    }

    public float GetTimeInSeconds()
    {
        return currentTimeOfDay * 86400f;
    }
}
