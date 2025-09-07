using UnityEngine;

public class BottleScorer : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Botella"))
        {
            // Suma puntos y botellas
            FindObjectOfType<GameManager>().AddBottle();
        }
    }
}
