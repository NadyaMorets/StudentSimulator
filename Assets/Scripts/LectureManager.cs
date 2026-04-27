using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LectureManager : MonoBehaviour
{
    [SerializeField] private GameObject choicePanel;
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TextMeshProUGUI resultText;

    private string selectedAction;
    private bool isInLecture = false;
    private Vector2 lastMoveInput; // Для сохранения последнего ввода

    // Базовые эффекты действий
    private Dictionary<string, (float energy, float rep, float mood)> actionEffects =
        new Dictionary<string, (float, float, float)>()
    {
        {"Listen", (0, 5, 10)},
        {"TakeNotes", (0, 10, 15)},
        {"Eat", (10, 0, 5)},
        {"Sleep", (5, 0, 0)},
        {"Leave", (0, -15, 15)}
    };

    // Рандомные исходы
    private Dictionary<string, List<(string text, float rep, float mood, float energy, float chance)>> outcomes =
        new Dictionary<string, List<(string, float, float, float, float)>>()
    {
        {"Sleep", new List<(string, float, float, float, float)>
        {
            ("Никто не заметил", 0, 0, 0, 0.25f),
            ("Препод сделал фото и отправил в чат группы", -10, 0, 0, 0.25f),
            ("Вы громко храпели, вся группа смотрит", -15, -10, 0, 0.25f),
            ("Приснился ответ на билет, вы проснулись с инсайтом", 0, 5, 0, 0.25f)
        }},
        {"Eat", new List<(string, float, float, float, float)>
        {
            ("Никто не заметил", 0, 0, 0, 0.25f),
            ("Препод сказал: 'На перемене ешьте!'", -5, -5, 0, 0.25f),
            ("Вы уронили бутерброд, все обернулись", -5, -5, 0, 0.25f),
            ("Угостили препода печеньем", 15, 10, 0, 0.25f)
        }},
        {"TakeNotes", new List<(string, float, float, float, float)>
        {
            ("Препод в восторге", 10, 15, 0, 0.34f),
            ("Вы случайно нарисовали мем, а не конспект", 0, 10, 0, 0.33f),
            ("Заметки помогли ответить на вопрос", 0, 15, 0, 0.33f)
        }}
    };

    private void Start()
    {
        if (choicePanel != null) choicePanel.SetActive(false);
        if (resultPanel != null) resultPanel.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isInLecture)
        {
            choicePanel.SetActive(true);
            isInLecture = true;
            Time.timeScale = 0f;

            if (CharacterInput.Current != null)
            {
                CharacterInput.Current.StopControlling();
            }
        }
    }

    public void OnActionSelected(string action)
    {
        selectedAction = action;
        choicePanel.SetActive(false);

        if (actionEffects.ContainsKey(action) && GameManager.Instance != null)
        {
            var effects = actionEffects[action];
            GameManager.Instance.AddEnergy(effects.energy);
            GameManager.Instance.AddReputation(effects.rep);
            GameManager.Instance.AddMood(effects.mood);
        }

        ShowResult();

        if (GameTime.Instance != null)
            GameTime.Instance.AddHours(1.5f);
    }

    private void ShowResult()
    {
        string resultMessage = "";

        switch (selectedAction)
        {
            case "Listen": resultMessage = "Вы слушали лекцию.\n"; break;
            case "TakeNotes": resultMessage = "Вы делали заметки.\n"; break;
            case "Eat": resultMessage = "Вы поели на паре.\n"; break;
            case "Sleep": resultMessage = "Вы поспали на паре.\n"; break;
            case "Leave": resultMessage = "Вы ушли с пары.\n"; break;
        }

        if (outcomes.ContainsKey(selectedAction))
        {
            var possibleOutcomes = outcomes[selectedAction];
            float totalChance = 0f;
            foreach (var o in possibleOutcomes) totalChance += o.chance;

            float random = Random.Range(0f, totalChance);
            float current = 0f;

            foreach (var outcome in possibleOutcomes)
            {
                current += outcome.chance;
                if (random <= current)
                {
                    resultMessage += $"\n{outcome.text}\n";

                    if (GameManager.Instance != null)
                    {
                        GameManager.Instance.AddReputation(outcome.rep);
                        GameManager.Instance.AddMood(outcome.mood);
                        GameManager.Instance.AddEnergy(outcome.energy);

                        if (outcome.rep != 0) resultMessage += $"Репутация: {(outcome.rep > 0 ? "+" : "")}{outcome.rep}\n";
                        if (outcome.mood != 0) resultMessage += $"Настроение: {(outcome.mood > 0 ? "+" : "")}{outcome.mood}\n";
                        if (outcome.energy != 0) resultMessage += $"Энергия: {(outcome.energy > 0 ? "+" : "")}{outcome.energy}\n";
                    }
                    break;
                }
            }
        }

        resultText.text = resultMessage;
        resultPanel.SetActive(true);
    }

    public void CloseResult()
    {
        resultPanel.SetActive(false);
        isInLecture = false;
        Time.timeScale = 1f;

        if (CharacterInput.Current != null)
        {
            CharacterInput.Current.ResetInputState();
            CharacterInput.Current.StartControlling();
        }
    }
}