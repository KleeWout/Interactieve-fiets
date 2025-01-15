using UnityEngine;

public class GameController : MonoBehaviour
{
    private WebSocketClient webSocketClient;

    // Called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {


        GameObject webSocketObject = GameObject.Find("WebSocket");
        if (webSocketObject != null)
        {
            webSocketClient = webSocketObject.GetComponent<WebSocketClient>();
            if (webSocketClient != null)
            {
                // Send a message to the WebSocket
                Debug.Log("Sending game started message to the WebSocket server...");
                Debug.Log(webSocketClient);
                WebSocketClient.Instance.SendMessage("{\"gameState\": \"started\"}");
            }
            else
            {
                Debug.LogError("WebSocketClient script not found on the WebSocket object.");
            }
        }
        else
        {
            Debug.LogError("WebSocket object not found under PersistentManager.");
        }
    }

    // Restart the current game scene
    public static void RestartGame()
    {
        var currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        UnityEngine.SceneManagement.SceneManager.LoadScene(currentScene.buildIndex);
    }
}
