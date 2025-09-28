using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject hudUI;       // todo el HUD
    public GameObject victoriaUI;  // panel victoria
    public GameObject derrotaUI;   // panel derrota

    public void MostrarVictoria()
    {
        hudUI.SetActive(false);
        victoriaUI.SetActive(true);
        derrotaUI.SetActive(false);
    }

    public void MostrarDerrota()
    {
        hudUI.SetActive(false);
        derrotaUI.SetActive(true);
        victoriaUI.SetActive(false);
    }
}
