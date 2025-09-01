using UnityEngine;

public class PatrolX : MonoBehaviour
{
    [Header("Límites en X (mundo)")]
    public float minX = -358f;
    public float maxX = 281f;

    [Header("Movimiento")]
    public float speed = 5f;
    public bool useLocalPosition = false;
    public bool startAtMin = true;

    bool dead;                 // si muere, deja de moverse
    Animator anim;
    Rigidbody rootRb;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rootRb = GetComponent<Rigidbody>(); // suele estar kinematic
        if (minX > maxX) { var t = minX; minX = maxX; maxX = t; }
    }

    int dir = 1;

    void Start()
    {
        if (startAtMin) SetX(minX);
        else dir = (GetX() >= (minX + maxX) * 0.5f) ? -1 : 1;
    }

    void Update()
    {
        if (dead) return;

        float x = GetX() + dir * speed * Time.deltaTime;
        if (dir > 0 && x >= maxX) { x = maxX; dir = -1; }
        else if (dir < 0 && x <= minX) { x = minX; dir = 1; }
        SetX(x);
    }

    // --- LLAMAR ESTO CUANDO LO GOLPEA LA BALA ---
    public void KillAndRagdoll(Vector3 hitPoint, Vector3 impulse)
    {
        if (dead) return;
        dead = true;              // deja de moverse

        // apagar animación del root
        if (anim) anim.enabled = false;

        // activar física en los huesos (ragdoll)
        var bodies = GetComponentsInChildren<Rigidbody>(true);
        Rigidbody closest = null; float best = float.MaxValue;

        foreach (var rb in bodies)
        {
            if (rb == rootRb) continue;   // no el root
            rb.isKinematic = false;
            rb.useGravity = true;

            float d = (rb.worldCenterOfMass - hitPoint).sqrMagnitude;
            if (d < best) { best = d; closest = rb; }
        }

        // habilitar colliders de huesos por si estaban apagados
        foreach (var col in GetComponentsInChildren<Collider>(true))
            if (!rootRb || col.attachedRigidbody != rootRb)
                col.enabled = true;

        if (closest) closest.AddForceAtPosition(impulse, hitPoint, ForceMode.Impulse);
    }

    // fallback por si querés detectar desde acá (si el root recibe la colisión)
    void OnCollisionEnter(Collision c)
    {
        if (!dead && c.collider.CompareTag("Colisionador"))
        {
            Vector3 p = c.GetContact(0).point;
            Vector3 imp = (c.rigidbody ? c.relativeVelocity * c.rigidbody.mass : Vector3.down * 5f);
            KillAndRagdoll(p, imp);
        }
    }

    float GetX() => useLocalPosition ? transform.localPosition.x : transform.position.x;
    void SetX(float x)
    {
        if (useLocalPosition) { var p = transform.localPosition; p.x = x; transform.localPosition = p; }
        else { var p = transform.position; p.x = x; transform.position = p; }
    }
}
