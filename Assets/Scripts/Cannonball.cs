using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Cannonball : MonoBehaviour
{
    [Tooltip("Impulso adicional al rigidbody impactado (adem�s del choque f�sico).")]
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
        
        if (extraImpactImpulse > 0f && col.rigidbody)
        {
            Vector3 dir = rb.velocity.sqrMagnitude > 0.01f ? rb.velocity.normalized : transform.forward;
            Vector3 point = col.contacts[0].point;
            col.rigidbody.AddForceAtPosition(dir * extraImpactImpulse, point, ForceMode.Impulse);
        }

       
        var patrol = col.collider.GetComponentInParent<PatrolX>();
        if (patrol != null)
        {
            var contact = col.GetContact(0);
            Vector3 dir = rb.velocity.sqrMagnitude > 0.01f ? rb.velocity.normalized : transform.forward;
            Vector3 impulse = rb.velocity * rb.mass + dir * extraImpactImpulse;
            patrol.KillAndRagdoll(contact.point, impulse);
        }

      
    }
}
