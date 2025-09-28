using Firebase;
using Firebase.Database;
using UnityEngine;

public class FirebaseTest : MonoBehaviour
{
    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                Debug.Log("Firebase listo!");
                DatabaseReference db = FirebaseDatabase.DefaultInstance.RootReference;
                db.Child("prueba").SetValueAsync("Hola mundo");
            }
            else
            {
                Debug.LogError("Firebase NO funciona");
            }
        });
    }
}