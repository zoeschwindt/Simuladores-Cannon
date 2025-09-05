using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ProjectileTelemetry : MonoBehaviour
{
    public string targetTagForHit = "Colisionador";   // tu tag de la bala en el objetivo NO importa; la bala es esta
    public float lifeSeconds = 5f;                    // por si tu bala se autodestruye

    Rigidbody rb;
    int shotId;
    bool impactSent;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        shotId = ShotLogger.Instance ? ShotLogger.Instance.BeginShot() : 0;
        if (lifeSeconds > 0f) Destroy(gameObject, lifeSeconds);
    }

    void OnCollisionEnter(Collision c)
    {
        // Registrar primer impacto
        if (!impactSent && ShotLogger.Instance)
        {
            ShotLogger.Instance.RecordFirstImpact(shotId, c);
            impactSent = true;
        }

        // Si el objeto impactado tiene un reporter, anotarle este shotId
        var rep = c.collider.GetComponentInParent<TargetKnockdownReporter>();
        if (rep) rep.MarkLastHitBy(shotId);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!impactSent && ShotLogger.Instance)
        {
            // crear una "colisión falsa" mínima si querés, o registrar solo el tiempo
            var fakeCollision = new Collision();
            ShotLogger.Instance.RecordFirstImpact(shotId, fakeCollision);
            impactSent = true;
        }

        var rep = other.GetComponentInParent<TargetKnockdownReporter>();
        if (rep) rep.MarkLastHitBy(shotId);
    }



    void OnDestroy()
    {
        if (ShotLogger.Instance) ShotLogger.Instance.EndShot(shotId);
    }
}
