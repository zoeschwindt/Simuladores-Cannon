using UnityEngine;

public class PersonHitDetector : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Colisionador"))
        {
            // Buscar el GameManager y restar 30 segundos
            FindObjectOfType<CountdownTimer>().SubtractTime(30f);
        }
    }
}
