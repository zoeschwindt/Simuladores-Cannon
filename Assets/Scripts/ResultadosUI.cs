using UnityEngine;
using UnityEngine.UI;

public class ResultadosUI : MonoBehaviour
{
    [Header("UI")]
    public GameObject resultadosPanel;
    public Transform contentParent;      // el Content del ScrollView
    public GameObject resultadoPrefab;   // prefab de texto para cada disparo

    void Start()
    {
        if (resultadosPanel) resultadosPanel.SetActive(false);
    }

    public void MostrarResultados()
    {
        if (!FirebaseManager.Instance || !FirebaseManager.Instance.isReady)
        {
            Debug.LogWarning(" Firebase no está listo todavía");
            return;
        }

        resultadosPanel.SetActive(true);
        RefrescarResultados();
    }

    private bool isRefreshing = false;

    public void RefrescarResultados()
    {
        if (!FirebaseManager.Instance || !FirebaseManager.Instance.isReady) return;
        if (isRefreshing) return; // <- evita duplicados por doble llamada

        isRefreshing = true;

        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        FirebaseManager.Instance.ObtenerResultados((lista) =>
        {
            if (lista.Count == 0)
            {
                var go = Instantiate(resultadoPrefab, contentParent);
                go.GetComponent<Text>().text = "No hay disparos guardados.";
            }
            else
            {
                foreach (var r in lista)
                {
                    string texto =
                        $"Ángulo: {r.angulo}\n" +
                        $"Fuerza: {r.fuerza}\n" +
                        $"Masa: {r.masa}\n" +
                        $"Acierto: {r.acierto}\n" +
                        $"Distancia: {r.distancia:F2}\n" +
                        $"Derribados: {r.derribados}";

                    var go = Instantiate(resultadoPrefab, contentParent);
                    go.GetComponent<Text>().text = texto;
                }
            }

            isRefreshing = false; // <- habilita nuevo refresco
        });
    }
    public void CerrarPanel()
    {
        if (resultadosPanel) resultadosPanel.SetActive(false);
    }
}
