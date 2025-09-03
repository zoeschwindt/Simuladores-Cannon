using UnityEngine;
using System.Collections;

[RequireComponent(typeof(HingeJoint), typeof(Rigidbody))]

public class KnockDownAndStay : MonoBehaviour
{
    [Header("Detección")]
    public string bulletTag = "Colisionador"; 

    [Header("Límites del hinge (grados)")]
    public float minAngle = -120f; 
    public float maxAngle = 0f;    

    [Header("Caída")]
    public float fallKickTorque = 25f; 

    [Header("Retorno")]
    public float returnDelay = 1.2f;   
    public float returnSpring = 250f;  
    public float returnDamper = 40f;   

    [Header("Sujeción una vez erguido")]
    public float holdSpring = 80f;     
    public float holdDamper = 8f;      

    HingeJoint hj;
    Rigidbody rb;
    bool returning;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        hj = GetComponent<HingeJoint>();

        rb.useGravity = true;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

       
        hj.useLimits = true;
        var lim = hj.limits;
        lim.min = minAngle; lim.max = maxAngle;
        lim.bounciness = 0f; lim.contactDistance = 0f;
        hj.limits = lim;

        
        var s = hj.spring;
        s.spring = holdSpring;
        s.damper = holdDamper;
        s.targetPosition = 0f;
        hj.spring = s;
        hj.useSpring = true;
        hj.useMotor = false;
    }

    void OnCollisionEnter(Collision c)
    {
        if (returning) return;
        if (!IsBullet(c)) return;

      
        hj.useSpring = false;

        
        rb.AddTorque(transform.right * fallKickTorque, ForceMode.Impulse);

        StartCoroutine(ReturnAfterDelay());
    }

    IEnumerator ReturnAfterDelay()
    {
        returning = true;
        yield return new WaitForSeconds(returnDelay);

        
        var s = hj.spring;
        s.spring = returnSpring;
        s.damper = returnDamper;
        s.targetPosition = 0f;
        hj.spring = s;
        hj.useSpring = true;

       
        while (Mathf.Abs(hj.angle) > 1.0f)
            yield return null;

        s.spring = holdSpring;
        s.damper = holdDamper;
        hj.spring = s;                 
        returning = false;
    }

    bool IsBullet(Collision c)
    {
        if (c.collider.CompareTag(bulletTag)) return true;
        if (c.rigidbody && c.rigidbody.CompareTag(bulletTag)) return true;
        if (c.transform.CompareTag(bulletTag)) return true;
        if (c.transform.root && c.transform.root.CompareTag(bulletTag)) return true;
        return false;
    }
}