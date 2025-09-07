using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Cannonball : MonoBehaviour
{
    [Tooltip("Impulso adicional al rigidbody impactado (además del choque físico).")]
    public float extraImpactImpulse = 0f;

    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void OnCollisionEnter(Collision col)
    {
        // tu empuje extra si el otro tiene RB
        if (extraImpactImpulse > 0f && col.rigidbody)
        {
            Vector3 dir = rb.velocity.sqrMagnitude > 0.01f ? rb.velocity.normalized : transform.forward;
            Vector3 point = col.contacts[0].point;
            col.rigidbody.AddForceAtPosition(dir * extraImpactImpulse, point, ForceMode.Impulse);
        }

        // --- avisar al enemigo para que caiga y deje de moverse (si tiene PatrolX) ---
        var patrol = col.collider.GetComponentInParent<PatrolX>();
        if (patrol != null)
        {
            var contact = col.GetContact(0);
            Vector3 dir = rb.velocity.sqrMagnitude > 0.01f ? rb.velocity.normalized : transform.forward;
            Vector3 impulse = rb.velocity * rb.mass + dir * extraImpactImpulse;
            patrol.KillAndRagdoll(contact.point, impulse);
        }

        // destruir la bala para que no rebote
        //Destroy(gameObject);
    }
}
