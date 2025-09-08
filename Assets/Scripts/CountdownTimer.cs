using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    [Header("Tiempo en segundos")]
    public float startTime = 60f; 

    [Header("Referencias UI")]
    public TextMeshProUGUI timerText;
    public GameObject losePanel;

    [Header("Audio")]
    public AudioSource backgroundMusic; 
    public AudioSource loseMusic;       

    float currentTime;
    bool isRunning = true;

    void Start()
    {
        currentTime = startTime;
        losePanel.SetActive(false);

        if (backgroundMusic != null)
            backgroundMusic.Play();
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

    
        timerText.text = string.Format("{0}:{1:00}", minutes, seconds);
    }

    public void ShowLosePanel()
    {
        losePanel.SetActive(true);
        Time.timeScale = 0f;

        
        if (backgroundMusic != null)
            backgroundMusic.Stop();

        if (loseMusic != null)
            loseMusic.Play();
    }

    
    public void SubtractTime(float amount)
    {
        currentTime -= amount;
        if (currentTime < 0)
            currentTime = 0;
    }
}
