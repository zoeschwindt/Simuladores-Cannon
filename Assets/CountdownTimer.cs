using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    [Header("Tiempo en segundos")]
    public float startTime = 60f; // tiempo inicial (ej: 1 minuto)

    [Header("Referencias UI")]
    public TextMeshProUGUI timerText;
    public GameObject losePanel;

    float currentTime;
    bool isRunning = true;

    void Start()
    {
        currentTime = startTime;
        losePanel.SetActive(false);
    }

    void Update()
    {
        if (!isRunning) return;

        currentTime -= Time.deltaTime;

        if (currentTime <= 0)
        {
            currentTime = 0;
            isRunning = false;
            ShowLosePanel();
        }

        UpdateTimerUI();
    }

    void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);

        // minutos sin cero adelante, segundos con 2 dígitos
        timerText.text = string.Format("{0}:{1:00}", minutes, seconds);
    }

    public void ShowLosePanel()
    {
        losePanel.SetActive(true);
        Time.timeScale = 0f; // pausa el juego
    }

    // 👉 Método para restar tiempo desde otro script
    public void SubtractTime(float amount)
    {
        currentTime -= amount;
        if (currentTime < 0)
            currentTime = 0;
    }
}
