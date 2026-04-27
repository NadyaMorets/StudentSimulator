using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField]
    private float currentEnergy = 80f;

    [SerializeField]
    private float currentReputation = 50f;

    [SerializeField]
    private float currentMood = 70f;

    public float CurrentEnergy
    {
        get => currentEnergy;
        set
        {
            currentEnergy = Mathf.Clamp(value, 0, 100);
            OnEnergyChanged?.Invoke(currentEnergy);
        }
    }

    public float CurrentReputation
    {
        get => currentReputation;
        set
        {
            currentReputation = Mathf.Clamp(value, 0, 100);
            OnReputationChanged?.Invoke(currentReputation);
        }
    }

    public float CurrentMood
    {
        get => currentMood;
        set
        {
            currentMood = Mathf.Clamp(value, 0, 100);
            OnMoodChanged?.Invoke(currentMood);
        }
    }

    public System.Action<float> OnEnergyChanged;
    public System.Action<float> OnReputationChanged;
    public System.Action<float> OnMoodChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddEnergy(float amount) => CurrentEnergy += amount;
    public void AddReputation(float amount) => CurrentReputation += amount;
    public void AddMood(float amount) => CurrentMood += amount;
}