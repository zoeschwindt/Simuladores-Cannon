using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance { get; private set; }

    private DatabaseReference reference;
    public bool isReady = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Inicializar Firebase
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                var options = new AppOptions
                {
                    DatabaseUrl = new System.Uri("https://simuladores-cannon-default-rtdb.firebaseio.com/")
                };

                FirebaseApp app = FirebaseApp.Create(options);
                reference = FirebaseDatabase.GetInstance(app).RootReference;

                isReady = true;
                Debug.Log(" Firebase inicializado correctamente con URL");
            }
            else
            {
                Debug.LogError(" No se pudieron resolver las dependencias de Firebase: " + task.Result);
            }
        });
    }

    public async void GuardarDisparo(string angulo, float fuerza, float masa, bool acierto, float distancia, int derribados)
    {
        if (!isReady || reference == null)
        {
            Debug.LogWarning(" Firebase no está listo todavía, no se guardó el disparo.");
            return;
        }

        var datos = new Dictionary<string, object>
        {
            { "angulo", angulo },
            { "fuerza", fuerza },
            { "masa", masa },
            { "acierto", acierto },
            { "distancia", distancia },
            { "derribados", derribados },
            { "timestamp", ServerValue.Timestamp }
        };

        try
        {
            await reference.Child("disparos").Push().SetValueAsync(datos);
            Debug.Log(" Disparo guardado en Firebase con éxito");
        }
        catch (System.Exception e)
        {
            Debug.LogError(" Error al guardar disparo en Firebase: " + e.Message);
        }
    }

    [System.Serializable]
    public class DisparoData
    {
        public string angulo;
        public float fuerza;
        public float masa;
        public bool acierto;
        public float distancia;
        public int derribados;
    }

    //  Obtener disparos ordenados (últimos primero)
    public void ObtenerResultados(System.Action<List<DisparoData>> callback)
    {
        if (!isReady || reference == null)
        {
            Debug.LogWarning(" Firebase no está listo para leer.");
            callback(new List<DisparoData>());
            return;
        }

        reference.Child("disparos").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            var lista = new List<DisparoData>();
            if (task.IsCompleted && task.Result.Exists)
            {
                foreach (var child in task.Result.Children)
                {
                    var dict = child.Value as Dictionary<string, object>;
                    if (dict != null)
                    {
                        var d = new DisparoData
                        {
                            angulo = dict.ContainsKey("angulo") ? dict["angulo"].ToString() : "0",
                            fuerza = dict.ContainsKey("fuerza") ? float.Parse(dict["fuerza"].ToString()) : 0f,
                            masa = dict.ContainsKey("masa") ? float.Parse(dict["masa"].ToString()) : 0f,
                            acierto = dict.ContainsKey("acierto") ? (bool)dict["acierto"] : false,
                            distancia = dict.ContainsKey("distancia") ? float.Parse(dict["distancia"].ToString()) : 0f,
                            derribados = dict.ContainsKey("derribados") ? int.Parse(dict["derribados"].ToString()) : 0
                        };
                        lista.Add(d);
                    }
                }
            }

            //  Mostrar más nuevos primero
            lista.Reverse();

            callback(lista);
        });
    }
}
