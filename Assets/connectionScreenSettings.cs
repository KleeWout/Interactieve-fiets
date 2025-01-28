using UnityEngine;
using System.IO;
using TMPro;
using System.Diagnostics;

public class connectionScreenSettings : MonoBehaviour
{
    [System.Serializable]
    public class Settings
    {
        public string WifiNetworkName;
    }
    public TMP_Text network;

    private string jsonFileName = "settings.json";
    private string jsonFilePath;

    void Start()
    {
        // Get the path to the directory containing the game executable
        jsonFilePath = Path.Combine(Application.dataPath, ".." ,jsonFileName);

        // Check if the JSON file exists in the same folder as the game executable
        if (File.Exists(jsonFilePath))
        {
            // Load the JSON file from the same folder as the game executable
            string jsonString = File.ReadAllText(jsonFilePath);
            Settings settings = JsonUtility.FromJson<Settings>(jsonString);
            network.text = $"Verbind met het WifiNetwerk \"{settings.WifiNetworkName}\"";

            // Use the JSON data as needed
            // For example, you can parse it into a class or dictionary
        }

        string exePath = Path.Combine(Application.dataPath, "webserver.exe");
        Process.Start(exePath);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
