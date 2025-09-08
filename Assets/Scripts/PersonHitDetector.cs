using UnityEngine;

public class PersonHitDetector : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Colisionador"))
        {
           
            FindObjectOfType<CountdownTimer>().SubtractTime(30f);
        }
    }
}
