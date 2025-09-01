// Assets/CharacterLocalGravity.cs
using UnityEngine;

public class CharacterLocalGravity : MonoBehaviour
{
    [Tooltip("Aceleración de 'gravedad' solo para este personaje (m/s²).")]
    public Vector3 customGravity = new Vector3(0f, -30f, 0f); // más negativo = cae más rápido

    [Tooltip("Desactivar la gravedad builtin de Unity en estos rigidbodies para evitar doble efecto.")]
    public bool disableBuiltinGravity = true;

    Rigidbody[] rbs;

    void Awake()
    {
        // Cachea TODOS los rigidbodies del personaje (incluye huesos del ragdoll)
        rbs = GetComponentsInChildren<Rigidbody>(true);

        if (disableBuiltinGravity)
            foreach (var rb in rbs) rb.useGravity = false; // así solo aplica esta gravedad local
    }

    void FixedUpdate()
    {
        // Aplica gravedad SOLO a los RBs que estén simulando (no-kinematic)
        foreach (var rb in rbs)
        {
            if (rb == null || rb.isKinematic) continue;
            rb.AddForce(customGravity, ForceMode.Acceleration);
        }
    }
}
