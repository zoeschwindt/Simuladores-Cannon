using UnityEngine;

public class PatrolX : MonoBehaviour
{
    [Header("Límites en X (mundo)")]
    public float minX = -358f;
    public float maxX = 281f;

    [Header("Movimiento")]
    [Tooltip("Unidades por segundo")]
    public float speed = 5f;
    [Tooltip("Si true usa posición local en lugar de mundo")]
    public bool useLocalPosition = false;
    [Tooltip("Comenzar en minX (si no, mantiene la posición actual)")]
    public bool startAtMin = true;

    int dir = 1; // 1 = hacia +X, -1 = hacia -X

    void Start()
    {
        if (minX > maxX) { var t = minX; minX = maxX; maxX = t; }

        if (startAtMin)
            SetX(minX);
        else
            dir = (GetX() >= (minX + maxX) * 0.5f) ? -1 : 1;
    }

    void Update()
    {
        float x = GetX() + dir * speed * Time.deltaTime;

        if (dir > 0 && x >= maxX) { x = maxX; dir = -1; }
        else if (dir < 0 && x <= minX) { x = minX; dir = 1; }

        SetX(x);
    }

    float GetX() =>
        useLocalPosition ? transform.localPosition.x : transform.position.x;

    void SetX(float x)
    {
        if (useLocalPosition)
        {
            var p = transform.localPosition;
            p.x = x;
            transform.localPosition = p;
        }
        else
        {
            var p = transform.position;
            p.x = x;
            transform.position = p;
        }
    }

#if UNITY_EDITOR
    // Dibuja los límites en la escena
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 a = transform.position; a.x = minX;
        Vector3 b = transform.position; b.x = maxX;
        Gizmos.DrawLine(a, b);
        Gizmos.DrawSphere(a, 0.2f);
        Gizmos.DrawSphere(b, 0.2f);
    }
#endif
}
