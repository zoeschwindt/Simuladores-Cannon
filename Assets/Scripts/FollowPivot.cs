using UnityEngine;

public class FollowPivot : MonoBehaviour
{
    public Transform target; 
    Vector3 localPos; Quaternion localRot;

    void Start()
    {
        localPos = target ? target.InverseTransformPoint(transform.position) : Vector3.zero;
        localRot = target ? Quaternion.Inverse(target.rotation) * transform.rotation : Quaternion.identity;
    }

    void LateUpdate()
    {
        if (!target) return;
        transform.position = target.TransformPoint(localPos);
        transform.rotation = target.rotation * localRot;
    }
}


