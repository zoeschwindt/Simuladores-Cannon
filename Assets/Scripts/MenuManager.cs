using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void PlayGame()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene("Nivel1");
    }

    public void QuitGame()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }

    public void Menu()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene("Menu");
    }
}
