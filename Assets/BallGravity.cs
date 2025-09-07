using UnityEngine;

public class BallGravity : MonoBehaviour
{
    [Header("Multiplicador de gravedad")]
    [Tooltip("1 = gravedad normal, 2 = el doble, 0.5 = la mitad")]
    public float gravityMultiplier = 1f;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true; // aseguramos que use gravedad
    }

    void FixedUpdate()
    {
        // Aplica la gravedad global multiplicada por el factor elegido en el Inspector
        rb.AddForce(Physics.gravity * gravityMultiplier, ForceMode.Acceleration);
    }
}
