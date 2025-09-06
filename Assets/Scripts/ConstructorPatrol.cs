using UnityEngine;

public class ConstructorPatrol : MonoBehaviour
{
    [Header("Límites en X (mundo)")]
    public float minX = -367f;   // límite izquierdo
    public float maxX = 316.2f;  // límite derecho

    [Header("Movimiento")]
    public float speed = 5f;

    bool dead;
    Animator anim;
    Rigidbody rootRb;
    int dir = -1;   // empieza yendo hacia la izquierda

    void Awake()
    {
        anim = GetComponent<Animator>();
        rootRb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        // arranca en el extremo derecho
        SetX(maxX);
        dir = -1;
        ApplyRotation();
    }

    void Update()
    {
        if (dead) return;

        float x = transform.position.x + dir * speed * Time.deltaTime;

        // --- límites ---
        if (dir < 0 && x <= minX)
        {
            x = minX;
            dir = 1;
            ApplyRotation();
        }
        else if (dir > 0 && x >= maxX)
        {
            x = maxX;
            dir = -1;
            ApplyRotation();
        }

        // --- mover ---
        Vector3 pos = transform.position;
        pos.x = x;
        transform.position = pos;
    }

    void ApplyRotation()
    {
        Vector3 euler = transform.eulerAngles;
        euler.y = (dir > 0) ? 88.803f : -88.803f;
        transform.eulerAngles = euler;
    }

    // --- Ragdoll al morir ---
    public void KillAndRagdoll(Vector3 hitPoint, Vector3 impulse)
    {
        if (dead) return;
        dead = true;

        if (anim) anim.enabled = false;

        var bodies = GetComponentsInChildren<Rigidbody>(true);
        Rigidbody closest = null; float best = float.MaxValue;

        foreach (var rb in bodies)
        {
            if (rb == rootRb) continue;
            rb.isKinematic = false;
            rb.useGravity = true;

            float d = (rb.worldCenterOfMass - hitPoint).sqrMagnitude;
            if (d < best) { best = d; closest = rb; }
        }

        foreach (var col in GetComponentsInChildren<Collider>(true))
            if (!rootRb || col.attachedRigidbody != rootRb)
                col.enabled = true;

        if (closest) closest.AddForceAtPosition(impulse, hitPoint, ForceMode.Impulse);
    }

    void OnCollisionEnter(Collision c)
    {
        if (!dead && c.collider.CompareTag("Colisionador"))
        {
            Vector3 p = c.GetContact(0).point;
            Vector3 imp = (c.rigidbody ? c.relativeVelocity * c.rigidbody.mass : Vector3.down * 5f);
            KillAndRagdoll(p, imp);
        }
    }

    void SetX(float x)
    {
        Vector3 pos = transform.position;
        pos.x = x;
        transform.position = pos;
    }
}
