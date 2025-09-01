using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class RagdollControl : MonoBehaviour
{
    [Tooltip("Tag del objeto que dispara el ragdoll al colisionar")]
    public string triggerTag = "Colisionador";

    Animator anim;
    Collider mainCollider;   // collider del ROOT
    Rigidbody mainRb;        // rigidbody del ROOT (kinematic)

    readonly List<Collider> ragdollColliders = new();
    readonly List<Rigidbody> ragdollRB = new();
    bool isRagdoll;

    void Awake()
    {
        anim = GetComponent<Animator>();
        mainCollider = GetComponent<Collider>();
        mainRb = GetComponent<Rigidbody>();

        // root kinematic para no moverse en modo "vivo"
        mainRb.isKinematic = true;
        mainRb.useGravity = false;

        CacheRagdollParts();
        SetRagdoll(false);   // >>> arrancar en IDLE
    }

    void CacheRagdollParts()
    {
        foreach (var c in GetComponentsInChildren<Collider>(true))
            if (c != mainCollider) ragdollColliders.Add(c);   // excluir el del root

        foreach (var rb in GetComponentsInChildren<Rigidbody>(true))
            if (rb != mainRb) ragdollRB.Add(rb);              // excluir el del root (si hubiera)
    }

    public void SetRagdoll(bool active)
    {
        isRagdoll = active;

        if (anim) anim.enabled = !active;     // Animator ON en vivo, OFF en ragdoll
        if (mainCollider) mainCollider.enabled = !active; // root collider solo en vivo

        // Huesos del ragdoll
        foreach (var rb in ragdollRB)
        {
            rb.isKinematic = !active;         // kinematic en vivo, dinámico en ragdoll
            rb.useGravity = active;
        }
        foreach (var c in ragdollColliders)
            c.enabled = active;               // colliders de huesos solo en ragdoll
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!isRagdoll && collision.collider.CompareTag(triggerTag))
        {
            // activar ragdoll
            SetRagdoll(true);

            // (Opcional) dar un pequeño impulso en el hueso más cercano al impacto
            Vector3 hit = collision.GetContact(0).point;
            Rigidbody closest = null; float best = float.MaxValue;
            foreach (var rb in ragdollRB)
            {
                float d = (rb.worldCenterOfMass - hit).sqrMagnitude;
                if (d < best) { best = d; closest = rb; }
            }
            if (closest != null)
            {
                Vector3 impulse = collision.relativeVelocity *
                                   (collision.rigidbody ? collision.rigidbody.mass : 1f);
                closest.AddForceAtPosition(impulse, hit, ForceMode.Impulse);
            }
        }
    }
}