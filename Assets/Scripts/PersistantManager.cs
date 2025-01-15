using UnityEngine;

public class PersistentManager : MonoBehaviour
{
    public static PersistentManager Instance;

    private void Awake()
    {
        // Check if an instance already exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Prevent this object from being destroyed on scene load
        }
        else
        {
            Destroy(gameObject); // Destroy any duplicate instances
        }
    }
}
