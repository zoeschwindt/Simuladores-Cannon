using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TargetKnockdownReporter : MonoBehaviour
{
    [Header("Identificación")]
    public string targetIdOverride;         

    [Header("Detección por Hinge")]
    public HingeJoint hinge;               
    public float downAngleThreshold = -60f; 
    public bool onlyOnce = true;

    [Header("Alternativa: detecto por destrucción del joint")]
    public bool considerJointBreakAsDown = true;

    int lastHitShotId = -1;
    bool reported;
    float lastAngle;

    void Reset() { hinge = GetComponent<HingeJoint>(); }

    public void MarkLastHitBy(int shotId) { lastHitShotId = shotId; }

    void Update()
    {
        if (reported) return;

        if (!hinge) hinge = GetComponent<HingeJoint>();

        bool isDown = false;
        if (hinge)
        {
            lastAngle = hinge.angle;
            if (lastAngle <= downAngleThreshold) isDown = true;
        }

        if (isDown)
        {
            Report();
            if (onlyOnce) reported = true;
        }
    }

    void OnJointBreak(float force)
    {
        if (considerJointBreakAsDown && !reported)
        {
            Report();
            if (onlyOnce) reported = true;
        }
    }

    void Report()
    {
        string id = string.IsNullOrEmpty(targetIdOverride) ? gameObject.name : targetIdOverride;
        if (ShotLogger.Instance)
        {
            
            int shotId = Mathf.Max(0, lastHitShotId);
            ShotLogger.Instance.NotifyTargetDown(shotId, id);
        }
    }
}
