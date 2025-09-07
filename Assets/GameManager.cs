using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI bottlesText;
    public TextMeshProUGUI pointsText;
    public GameObject winPanel;

    [Header("Objetivos")]
    public int targetBottles = 5;   // cuántas botellas hay que derribar

    int bottlesCount = 0;
    int points = 0;

    void Start()
    {
        winPanel.SetActive(false); // oculta el panel al inicio
        UpdateUI();
    }

    public void AddBottle()
    {
        bottlesCount++;
        points += 100;
        UpdateUI();

        if (bottlesCount >= targetBottles)
        {
            WinGame();
        }
    }

    void UpdateUI()
    {
        bottlesText.text = "Botellas: " + bottlesCount;
        pointsText.text = "Puntos: " + points;
    }

    void WinGame()
    {
        winPanel.SetActive(true);
        Time.timeScale = 0f; // pausa el juego
    }
}
