using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class RagdollControl : MonoBehaviour
{
    [Tooltip("Tag del objeto que dispara el ragdoll al colisionar")]
    public string triggerTag = "Colisionador";

    [Header("Audio")]
    public AudioSource impactSfx; 

    Animator anim;
    Collider mainCollider;   
    Rigidbody mainRb;        

    readonly List<Collider> ragdollColliders = new();
    readonly List<Rigidbody> ragdollRB = new();
    bool isRagdoll;

    void Awake()
    {
        anim = GetComponent<Animator>();
        mainCollider = GetComponent<Collider>();
        mainRb = GetComponent<Rigidbody>();

       
        mainRb.isKinematic = true;
        mainRb.useGravity = false;

        CacheRagdollParts();
        SetRagdoll(false);   
    }

    void CacheRagdollParts()
    {
        foreach (var c in GetComponentsInChildren<Collider>(true))
            if (c != mainCollider) ragdollColliders.Add(c); 

        foreach (var rb in GetComponentsInChildren<Rigidbody>(true))
            if (rb != mainRb) ragdollRB.Add(rb);           
    }

    public void SetRagdoll(bool active)
    {
        isRagdoll = active;

        if (anim) anim.enabled = !active;         
        if (mainCollider) mainCollider.enabled = !active; 

        foreach (var rb in ragdollRB)
        {
            rb.isKinematic = !active;              
            rb.useGravity = active;
        }

        foreach (var c in ragdollColliders)
            c.enabled = active;                     
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!isRagdoll && collision.collider.CompareTag(triggerTag))
        {
            
            SetRagdoll(true);

            
            Vector3 hit = collision.GetContact(0).point;
            Rigidbody closest = null;
            float best = float.MaxValue;

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

            
            if (impactSfx != null)
                impactSfx.Play(); 
        }
    }
}
