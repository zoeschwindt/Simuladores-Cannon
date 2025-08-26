using UnityEngine;
using UnityEngine.UI;

public class CannonController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] Transform baseTransform;   // Se desliza en X (mundo)
    [SerializeField] Transform barrelPivot;     // Pivot LIMPIO (0,0,0) en la bisagra
    [SerializeField] Transform muzzle;          // Boca del cañón (Z/forward apunta hacia afuera)

    [Header("UI")]
    [SerializeField] Slider horizontalSlider;   // 0..1
    [SerializeField] Slider verticalSlider;     // 0..1 (si usás Bottom-To-Top dejá invertVerticalSlider = false)
    [SerializeField] bool invertVerticalSlider = false;

    [Header("Rangos de movimiento")]
    [SerializeField] float minX = -10f;
    [SerializeField] float maxX = 10f;
    [SerializeField] float minElevation = 0f;   // grados
    [SerializeField] float maxElevation = 60f;  // grados

    public enum Axis { X, Y, Z }
    [SerializeField] Axis elevationAxis = Axis.X; // cambiá si tu bisagra es Y o Z

    [Header("Disparo")]
    [SerializeField] Rigidbody projectilePrefab;
    [SerializeField] float muzzleVelocity = 30f;
    [SerializeField] float fireCooldown = 0.25f;
    [SerializeField] float projectileLife = 8f;
    [SerializeField] float spawnForwardOffset = 0.3f; // empuja el spawn fuera del cañón
    [SerializeField] bool ignoreCollisionWithCannon = true;
    [SerializeField] AudioSource fireSfx;

    [Header("Comportamiento")]
    [SerializeField] bool startAtMinOnPlay = true; // clave para que NO se vaya para arriba

    float nextFireTime;
    Quaternion barrelZeroLocalRot; // "0°" del pivot al iniciar

    void Reset()
    {
        baseTransform = transform;
        barrelPivot = transform;
        muzzle = transform;
    }

    void Awake()
    {
        // Guardamos el "cero" del pivot tal como está en la escena
        barrelZeroLocalRot = barrelPivot ? barrelPivot.localRotation : Quaternion.identity;

        // Seteo de sliders y listeners
        if (horizontalSlider)
        {
            horizontalSlider.minValue = 0f;
            horizontalSlider.maxValue = 1f;
            horizontalSlider.onValueChanged.AddListener(SetHorizontalNormalized);
        }
        if (verticalSlider)
        {
            verticalSlider.minValue = 0f;
            verticalSlider.maxValue = 1f;
            verticalSlider.onValueChanged.AddListener(SetElevationNormalized);
        }
    }

    void Start()
    {
        // Sincroniza horizontal al estado actual del cañón (no dispara eventos)
        if (baseTransform && horizontalSlider)
        {
            float t = Mathf.InverseLerp(minX, maxX, baseTransform.position.x);
            horizontalSlider.SetValueWithoutNotify(Mathf.Clamp01(t));
            SetHorizontalNormalized(horizontalSlider.value);
        }

        // Fuerza a empezar en minElevation para que NO apunte para arriba
        if (startAtMinOnPlay)
        {
            if (verticalSlider)
                verticalSlider.SetValueWithoutNotify(invertVerticalSlider ? 1f : 0f);
            ApplyElevationAngle(minElevation);
        }
        else
        {
            // Si no forzamos, al menos aplicamos el valor actual del slider
            if (verticalSlider) SetElevationNormalized(verticalSlider.value);
        }
    }

    // --- Movimiento horizontal ---
    public void SetHorizontalNormalized(float t)
    {
        float x = Mathf.Lerp(minX, maxX, Mathf.Clamp01(t));
        if (!baseTransform) return;
        var p = baseTransform.position;
        p.x = x;
        baseTransform.position = p;
    }

    // --- Elevación ---
    void ApplyElevationAngle(float angle)
    {
        if (!barrelPivot) return;
        Vector3 axis = elevationAxis == Axis.X ? Vector3.right :
                       elevationAxis == Axis.Y ? Vector3.up : Vector3.forward;
        // Rotación local estable: base (cero) * giro en eje elegido
        barrelPivot.localRotation = barrelZeroLocalRot * Quaternion.AngleAxis(angle, axis);
    }

    public void SetElevationNormalized(float t)
    {
        if (invertVerticalSlider) t = 1f - t;
        float angle = Mathf.Lerp(minElevation, maxElevation, Mathf.Clamp01(t));
        ApplyElevationAngle(angle);
    }

    // --- Disparo ---
    public void Fire()
    {
        if (Time.time < nextFireTime) return;
        if (!projectilePrefab || !muzzle) return;

        nextFireTime = Time.time + fireCooldown;

        Vector3 spawnPos = muzzle.position + muzzle.forward * Mathf.Max(0f, spawnForwardOffset);
        Quaternion rot = muzzle.rotation;

        Rigidbody rb = Instantiate(projectilePrefab, spawnPos, rot);
        rb.velocity = muzzle.forward * muzzleVelocity;

        if (ignoreCollisionWithCannon)
        {
            var projCols = rb.GetComponentsInChildren<Collider>();
            var cannonCols = (baseTransform ? baseTransform : transform).GetComponentsInChildren<Collider>();
            foreach (var pc in projCols)
                foreach (var cc in cannonCols)
                    if (pc && cc) Physics.IgnoreCollision(pc, cc, true);
        }

        if (projectileLife > 0f) Destroy(rb.gameObject, projectileLife);
        if (fireSfx) fireSfx.Play();
    }

    // Controles de prueba (A/D, W/S, Space)
    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        if (horizontalSlider && Mathf.Abs(h) > 0.01f)
            horizontalSlider.value = Mathf.Clamp01(horizontalSlider.value + h * Time.deltaTime);
        if (verticalSlider && Mathf.Abs(v) > 0.01f)
            verticalSlider.value = Mathf.Clamp01(verticalSlider.value + v * Time.deltaTime);
        if (Input.GetKeyDown(KeyCode.Space)) Fire();
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (maxX < minX) maxX = minX;
        if (maxElevation < minElevation) maxElevation = minElevation;

        // Solo previsualización en editor (fuera de Play)
        if (!Application.isPlaying)
        {
            if (horizontalSlider) SetHorizontalNormalized(horizontalSlider.value);
            if (verticalSlider) SetElevationNormalized(verticalSlider.value);
        }
    }

    [ContextMenu("Calibrar: usar rotación actual como 0°")]
    void CalibrarCero()
    {
        if (!barrelPivot) return;
        barrelZeroLocalRot = barrelPivot.localRotation; // toma la pose actual como "horizontal"
        ApplyElevationAngle(minElevation);
        if (verticalSlider) verticalSlider.SetValueWithoutNotify(invertVerticalSlider ? 1f : 0f);
    }
#endif
}