using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ProjectileTelemetry : MonoBehaviour
{
    public string targetTagForHit = "Colisionador";   
    public float lifeSeconds = 5f;                    

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
    
        if (!impactSent && ShotLogger.Instance)
        {
            ShotLogger.Instance.RecordFirstImpact(shotId, c);
            impactSent = true;
        }

        var rep = c.collider.GetComponentInParent<TargetKnockdownReporter>();
        if (rep) rep.MarkLastHitBy(shotId);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!impactSent && ShotLogger.Instance)
        {
        
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
