using UnityEngine;
using UnityEngine.UI;

public class CannonController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] Transform baseTransform;   
    [SerializeField] Transform barrelPivot;    
    [SerializeField] Transform muzzle;         

    [Header("UI Ángulo/Horizontal")]
    [SerializeField] Slider horizontalSlider;   
    [SerializeField] Slider verticalSlider;     
    [SerializeField] bool invertVerticalSlider = false;
    [SerializeField] Text angleLabel;           

    [Header("UI Fuerza")]
    [SerializeField] Slider forceSlider;        
    [SerializeField] float minForce = 50f;
    [SerializeField] float maxForce = 1500f;
    [SerializeField] Text forceLabel;           

    [Header("UI Masa")]
    [SerializeField] Dropdown massDropdown;     
    [SerializeField] float[] massOptions = new float[] { 0.5f, 1f, 2f, 5f };
    [SerializeField] Text massLabel;           

    [Header("Rangos de movimiento")]
    [SerializeField] float minX = -10f;
    [SerializeField] float maxX = 10f;
    [SerializeField] float minElevation = 0f;   
    [SerializeField] float maxElevation = 60f;  

    public enum Axis { X, Y, Z }
    [SerializeField] Axis elevationAxis = Axis.X; 

    [Header("Disparo")]
    [SerializeField] Rigidbody projectilePrefab;
    [SerializeField] float muzzleVelocity = 300f;  
    [SerializeField] float fireCooldown = 0.25f;
    [SerializeField] float projectileLife = 8f;
    [SerializeField] float spawnForwardOffset = 0.3f; 
    [SerializeField] bool ignoreCollisionWithCannon = true;
    [SerializeField] AudioSource fireSfx;

    [Header("Comportamiento")]
    [SerializeField] bool startAtMinOnPlay = true;

    float nextFireTime;
    Quaternion barrelZeroLocalRot; 

    void Reset()
    {
        baseTransform = transform;
        barrelPivot = transform;
        muzzle = transform;
    }

    void Awake()
    {
        
        barrelZeroLocalRot = barrelPivot ? barrelPivot.localRotation : Quaternion.identity;

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

       
        if (forceSlider)
        {
            forceSlider.minValue = minForce;
            forceSlider.maxValue = maxForce;
            
            forceSlider.SetValueWithoutNotify(Mathf.Clamp(muzzleVelocity, minForce, maxForce));
            forceSlider.onValueChanged.AddListener(v =>
            {
                muzzleVelocity = v;
                if (forceLabel) forceLabel.text = $"Fuerza: {muzzleVelocity:0}";
            });
            if (forceLabel) forceLabel.text = $"Fuerza: {muzzleVelocity:0}";
        }

        if (massDropdown && massDropdown.options.Count == 0)
        {
            massDropdown.options.Clear();
            foreach (var m in massOptions)
                massDropdown.options.Add(new Dropdown.OptionData($"{m:0.##} kg"));
            massDropdown.value = Mathf.Clamp(massDropdown.value, 0, Mathf.Max(0, massOptions.Length - 1));
            massDropdown.RefreshShownValue();
        }
        if (massDropdown)
        {
            massDropdown.onValueChanged.AddListener(_ =>
            {
                if (massLabel) massLabel.text = $"Masa: {GetSelectedMass():0.##} kg";
            });
            if (massLabel) massLabel.text = $"Masa: {GetSelectedMass():0.##} kg";
        }
    }

    void Start()
    {
       
        if (baseTransform && horizontalSlider)
        {
            float t = Mathf.InverseLerp(minX, maxX, baseTransform.position.x);
            horizontalSlider.SetValueWithoutNotify(Mathf.Clamp01(t));
            SetHorizontalNormalized(horizontalSlider.value);
        }

        
        if (startAtMinOnPlay)
        {
            if (verticalSlider)
                verticalSlider.SetValueWithoutNotify(invertVerticalSlider ? 1f : 0f);
            ApplyElevationAngle(minElevation);
        }
        else
        {
            if (verticalSlider) SetElevationNormalized(verticalSlider.value);
        }

        
        if (angleLabel) angleLabel.text = $"Ángulo: {minElevation:0}°";
    }

    
    public void SetHorizontalNormalized(float t)
    {
        float x = Mathf.Lerp(minX, maxX, Mathf.Clamp01(t));
        if (!baseTransform) return;
        var p = baseTransform.position;
        p.x = x;
        baseTransform.position = p;
    }

    
    void ApplyElevationAngle(float angle)
    {
        if (!barrelPivot) return;
        Vector3 axis = elevationAxis == Axis.X ? Vector3.right :
                       elevationAxis == Axis.Y ? Vector3.up : Vector3.forward;

        barrelPivot.localRotation = barrelZeroLocalRot * Quaternion.AngleAxis(angle, axis);
        if (angleLabel) angleLabel.text = $"Ángulo: {angle:0}°";
    }

    public void SetElevationNormalized(float t)
    {
        if (invertVerticalSlider) t = 1f - t;
        float angle = Mathf.Lerp(minElevation, maxElevation, Mathf.Clamp01(t));
        ApplyElevationAngle(angle);
    }

   
    float GetSelectedMass()
    {
        if (!massDropdown || massOptions == null || massOptions.Length == 0) return 1f;
        int i = Mathf.Clamp(massDropdown.value, 0, massOptions.Length - 1);
        return massOptions[i];
    }

   
    public void Fire()
    {
        if (Time.time < nextFireTime) return;
        if (!projectilePrefab || !muzzle) return;

        nextFireTime = Time.time + fireCooldown;

        Vector3 spawnPos = muzzle.position + muzzle.forward * Mathf.Max(0f, spawnForwardOffset);
        Quaternion rot = muzzle.rotation;

        Rigidbody rb = Instantiate(projectilePrefab, spawnPos, rot);

       
        rb.mass = GetSelectedMass();

       
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

        if (!Application.isPlaying)
        {
            if (horizontalSlider) SetHorizontalNormalized(horizontalSlider.value);
            if (verticalSlider) SetElevationNormalized(verticalSlider.value);
            if (forceSlider)
            {
                forceSlider.minValue = minForce;
                forceSlider.maxValue = maxForce;
            }
        }
    }

    [ContextMenu("Calibrar: usar rotación actual como 0°")]
    void CalibrarCero()
    {
        if (!barrelPivot) return;
        barrelZeroLocalRot = barrelPivot.localRotation; 
        ApplyElevationAngle(minElevation);
        if (verticalSlider) verticalSlider.SetValueWithoutNotify(invertVerticalSlider ? 1f : 0f);
    }
#endif
}
