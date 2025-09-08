using UnityEngine;



public class BottleScorer : MonoBehaviour
{
    public AudioSource audioSource; 

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Botella"))
        {
            
            FindObjectOfType<GameManager>().AddBottle();

           
            if (audioSource != null)
            {
                audioSource.Play();
            }
        }
    }
}
