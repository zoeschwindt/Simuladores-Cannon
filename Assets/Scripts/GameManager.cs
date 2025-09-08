using UnityEngine;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI bottlesText;
    public TextMeshProUGUI pointsText;
    public TextMeshProUGUI pointsPopupText;
    public GameObject winPanel;

    [Header("Objetivos")]
    public int targetBottles = 5;

    [Header("Audio")]
    public AudioSource backgroundMusic;  
    public AudioSource winMusic;         

    int bottlesCount = 0;
    int points = 0;

    void Start()
    {
        winPanel.SetActive(false);
        pointsText.text = "";
        pointsPopupText.gameObject.SetActive(false);
        bottlesText.text = bottlesCount.ToString();

        if (backgroundMusic != null)
            backgroundMusic.Play();  
    }

    public void AddBottle()
    {
        bottlesCount++;
        points += 100;

        bottlesText.text = bottlesCount.ToString();
        pointsText.text = points.ToString();

        StartCoroutine(ShowPointsPopup("+100", 1.0f));

        if (bottlesCount >= targetBottles)
        {
            WinGame();
        }
    }

    IEnumerator ShowPointsPopup(string text, float duration)
    {
        pointsPopupText.text = text;
        pointsPopupText.gameObject.SetActive(true);

        yield return new WaitForSeconds(duration);

        pointsPopupText.gameObject.SetActive(false);
    }

    void WinGame()
    {
        winPanel.SetActive(true);
        Time.timeScale = 0f; 

        
        if (backgroundMusic != null)
            backgroundMusic.Stop();

        if (winMusic != null)
            winMusic.Play();
    }
}
