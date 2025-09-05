using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShotLogger : MonoBehaviour
{
    [System.Serializable]
    public class Attempt
    {
        public int id;
        public float startTime;
        public bool firstImpactRecorded;
        public float timeOfFlight;
        public Vector3 impactPoint;
        public Vector3 relativeVelocity;
        public float collisionImpulse;
        public HashSet<string> knockedTargets = new HashSet<string>();
    }

    public static ShotLogger Instance { get; private set; }

    [Header("Scoring")]
    public int pointsPerTarget = 100;
    public float timePenaltyPerSecond = 3f;    // resta puntos por segundo de vuelo
    public float impulseBonusScale = 0.2f;     // bonus = impulseMagnitude * scale
    public float minReportDuration = 2.5f;     // tiempo mínimo que se mantiene visible el panel

    [Header("UI")]
    public GameObject reportPanel;
    public Text scoreText;
    public Text detailsText;

    int _nextId = 1;
    readonly Dictionary<int, Attempt> _attempts = new();

    void Awake()
    {
        if (Instance && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        if (reportPanel) reportPanel.SetActive(false);
    }

    // ---- API llamada por la bala / targets ----
    public int BeginShot()
    {
        int id = _nextId++;
        _attempts[id] = new Attempt { id = id, startTime = Time.time };
        return id;
    }

    public void RecordFirstImpact(int shotId, Collision c)
    {
        if (!_attempts.TryGetValue(shotId, out var a) || a.firstImpactRecorded) return;

        a.firstImpactRecorded = true;
        a.timeOfFlight = Time.time - a.startTime;
        a.impactPoint = c.GetContact(0).point;
        a.relativeVelocity = c.relativeVelocity;
        a.collisionImpulse = c.impulse.magnitude;
    }

    public void NotifyTargetDown(int shotId, string targetId)
    {
        if (!_attempts.TryGetValue(shotId, out var a)) return;
        a.knockedTargets.Add(targetId);
    }

    public void EndShot(int shotId)
    {
        if (!_attempts.TryGetValue(shotId, out var a)) return;

        // ----- Score -----
        int score = a.knockedTargets.Count * pointsPerTarget;

        if (a.firstImpactRecorded)
        {
            score -= Mathf.RoundToInt(a.timeOfFlight * timePenaltyPerSecond);
            score += Mathf.RoundToInt(a.collisionImpulse * impulseBonusScale);
        }

        // ----- Reporte -----
        if (reportPanel && scoreText && detailsText)
        {
            reportPanel.SetActive(true);
            scoreText.text = $"Puntuación: <b>{Mathf.Max(0, score)}</b>";

            string impact = a.firstImpactRecorded ? $"({a.impactPoint.x:F2}, {a.impactPoint.y:F2}, {a.impactPoint.z:F2})" : "—";
            string relV = a.firstImpactRecorded ? $"{a.relativeVelocity.magnitude:F2} m/s" : "—";
            string impulse = a.firstImpactRecorded ? $"{a.collisionImpulse:F2} Ns" : "—";
            detailsText.text = $"Tiro #{a.id}\n" +
                               $"- Tiempo de vuelo: {a.timeOfFlight:F3} s\n" +
                               $"- Punto de impacto: {impact}\n" +
                               $"- Velocidad relativa: {relV}\n" +
                               $"- Impulso de colisión: {impulse}\n" +
                               $"- Piezas derribadas: {a.knockedTargets.Count} ({string.Join(", ", a.knockedTargets)})";


            StopAllCoroutines();
            StartCoroutine(HidePanelAfter(minReportDuration));
        }

        _attempts.Remove(shotId);
    }


    System.Collections.IEnumerator HidePanelAfter(float t)
    {
        yield return new WaitForSeconds(t);
        if (reportPanel) reportPanel.SetActive(false);
    }
}
