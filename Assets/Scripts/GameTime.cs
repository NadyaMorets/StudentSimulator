using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    private static GameTime instance;
    public static GameTime Instance => instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindAndSetTimeText();
    }

    private void FindAndSetTimeText()
    {
        GameObject timeTextObject = GameObject.FindGameObjectWithTag("TimeText");

        if (timeTextObject == null)
        {
            timeTextObject = GameObject.Find("TimeText");
        }

        if (timeTextObject != null)
        {
            timeText = timeTextObject.GetComponent<TextMeshProUGUI>();
            if (timeText != null)
            {
                UpdateTimeDisplay(); 
            }
        }
        else
        {
            Debug.LogWarning("TimeText UI не найден на сцене!");
        }
    }

    private void Start()
    {
        if (currentTimeOfDay == 0f)
        {
            currentTimeOfDay = startHour / 24f;
            UpdateTimeValues();
        }

        if (timeText == null)
        {
            FindAndSetTimeText();
        }
        else
        {
            UpdateTimeDisplay();
        }
    }

    private void Update()
    {
        currentTimeOfDay += Time.deltaTime / (dayLengthInMinutes * 60f);

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

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    public void AddHours(float hours)
    {
        currentTimeOfDay += hours / 24f;
        if (currentTimeOfDay >= 1f)
            currentTimeOfDay -= 1f;
        UpdateTimeValues();
        UpdateTimeDisplay();
    }
}