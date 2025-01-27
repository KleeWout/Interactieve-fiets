using UnityEngine;
using System.IO;
using TMPro;

public class connectionScreenSettings : MonoBehaviour
{
    [System.Serializable]
    public class Settings
    {
        public string WifiNetworkName;
    }


    public TMP_Text network;


    // void Start()
    // {
    //     // Load the JSON file from the Resources folder
    //     TextAsset jsonFile = Resources.Load<TextAsset>("settings");
    //     if (jsonFile != null)
    //     {
    //         // Parse the JSON file
    //         string jsonString = jsonFile.text;
    //         Settings settings = JsonUtility.FromJson<Settings>(jsonString);

    //         network.text = $"Verbind met het WifiNetwerk \"{settings.WifiNetworkName}\"";
    //     }
    //     else
    //     {
    //         Debug.LogError("Failed to load JSON file.");
    //     }
    // }

    private string jsonFileName = "wifiNetwork.json";
    private string jsonFilePath;

    void Start()
    {
        // Get the path to the directory containing the game executable
        jsonFilePath = Path.Combine(Application.dataPath, jsonFileName);

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
        else
        {
            Debug.LogError("JSON file not found in the same folder as the game executable.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
