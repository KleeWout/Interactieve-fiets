using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

using Models.GameModeModel;
using Models.WebSocketMessage;


public class WebSocketClient : MonoBehaviour
{
    public static WebSocketClient Instance;
    public string serverAddress = "localhost:8080";


    private ClientWebSocket webSocket = new ClientWebSocket();

    private Queue<string> sceneLoadRequests = new Queue<string>();
    private bool sceneChangeRequested = false;

    private GameSelect gameSelect;

    // public GameOverScreen GameOverScreen;

    private async void Start()
    {
        gameSelect = GetComponent<GameSelect>();
        Uri serverUri = new Uri($"ws://{serverAddress}");

        Debug.Log("Connecting to WebSocket server...");
        await webSocket.ConnectAsync(serverUri, CancellationToken.None);
        Debug.Log("Connected to WebSocket server!");

        // Start receiving messages
        ReceiveMessages();
        SendMessageToSocket(new WebSocketMessage());
    }

    private async void ReceiveMessages()
    {
        var buffer = new byte[1024 * 4];

        try
        {
            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    Debug.Log("Server closed connection.");
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                    break;
                }

                string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                // Debug.Log($"Received: {message}");
                JObject jsonObject = JObject.Parse(message);


                // Switch scenes based on the "gameMode" value in the JSON message
                if (jsonObject.ContainsKey("gameCode"))
                {
                    gameSelect.SetGameCode(jsonObject["gameCode"].ToString());
                }

                if (jsonObject.ContainsKey("gameMode"))
                {
                    string gameMode = jsonObject["gameMode"].ToString();
                    if (gameMode == "singleplayer")
                    {
                        Debug.Log("Queueing scene change: SinglePlayer");

                        // sceneLoadRequests.Enqueue("SinglePlayer");
                        // sceneChangeRequested = true;
                        gameSelect.SwitchGameMode(GameMode.SinglePlayer);
                    }
                    else if (gameMode == "multiplayer")
                    {
                        Debug.Log("Queueing scene change: MultiPlayer");
                        // sceneLoadRequests.Enqueue("MultiPlayer");
                        // sceneChangeRequested = true;
                        gameSelect.SwitchGameMode(GameMode.MultiPlayer);
                    }
                }

                if (jsonObject.ContainsKey("gameState"))
                {
                    string gameState = jsonObject["gameState"].ToString();
                    if (gameState == "stop")
                    {
                        // GameOverScreen.Setup();

                    }
                    if (gameState == "restart")
                    {
                        GameController.RestartGame();
                    }

                }

            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error receiving WebSocket messages: {ex.Message}");
        }
    }
    
    public async void SendMessageToSocket(WebSocketMessage data)
    {
        if (webSocket.State == WebSocketState.Open)
        {
            string json = JsonUtility.ToJson(data);
            var bytes = Encoding.UTF8.GetBytes(json);
            await webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
            // Debug.Log($"Sent: {json}");
        }
        else
        {
            Debug.LogError("WebSocket is not connected.");
        }
    }

    private void Update()
    {
        //scene switching moet gebeuren op main thread
        if (sceneChangeRequested && sceneLoadRequests.Count > 0)
        {
            string sceneName = sceneLoadRequests.Dequeue();
            Debug.Log($"Loading scene: {sceneName}");
            SceneManager.LoadScene(sceneName);
            sceneChangeRequested = false;
        }

        // Example: Send a message when the spacebar is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            WebSocketMessage message = new WebSocketMessage();
            SendMessageToSocket(message);
        }
    }

    private async void OnDestroy()
    {
        if (webSocket != null)
        {
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Unity closing connection", CancellationToken.None);
            webSocket.Dispose();
            Debug.Log("WebSocket connection closed.");
        }
    }
}