using UnityEngine;
using UnityEngine.SceneManagement;
public class GameController : MonoBehaviour
{
    private WebSocketClient webSocketClient;
    public GameOverScreen gameOverScreen;
    public string value;

    // Called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        GameObject webSocketObject = GameObject.Find("WebSocket");
        webSocketClient = webSocketObject.GetComponent<WebSocketClient>();
        Debug.Log("GameController Awake");
        webSocketClient.SendMessageToSocket("{\"gameState\": \"started\"}");

    }

    // Restart the current game scene
    public static void RestartGame()
    {
        var currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        UnityEngine.SceneManagement.SceneManager.LoadScene(currentScene.buildIndex);
    }

    public static void PlaySingleplayer()
    {
        SceneManager.LoadScene("Singleplayer");
    }
    public static void PlayMultiplayer()
    {
        SceneManager.LoadScene("Multiplayer");
    }
    public static void PlayMenu()
    {
        SceneManager.LoadScene("Menu");
    }
    public static void StopGame()
    {
        GameOverScreen.Setup();
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Y))
        {
            StopGame();
        }
    }
}
