using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine.Analytics;


public class WebSocketClient : MonoBehaviour
{
    public static WebSocketClient Instance;


    private ClientWebSocket webSocket;

    private Queue<string> sceneLoadRequests = new Queue<string>();
    private bool sceneChangeRequested = false;

    public GameOverScreen GameOverScreen;

    public TerrainGen TerrainGen;


    private async void Start()
    {
        webSocket = new ClientWebSocket();
        Uri serverUri = new Uri("ws://localhost:8080");
        Debug.Log("Connecting to WebSocket server...");
        await webSocket.ConnectAsync(serverUri, CancellationToken.None);
        Debug.Log("Connected to WebSocket server!");
        // Start receiving messages
        ReceiveMessages();
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
                Debug.Log($"Received: {message}");


                // Parse the JSON message
                // var jsonObject = JsonUtility.FromJson<Dictionary<string, string>>(message);
                JObject jsonObject = JObject.Parse(message);


                // Switch scenes based on the "gamemode" value in the JSON message

                if (jsonObject.ContainsKey("gamemode"))
                {
                    string gameMode = jsonObject["gamemode"].ToString();
                    if (gameMode == "singleplayer")
                    {
                        GameController.PlaySingleplayer();
                    }
                    else if (gameMode == "multiplayer")
                    {
                        GameController.PlayMultiplayer();
                    }
                    else if (gameMode == "menu")
                    {
                        GameController.PlayMenu();
                    }
                }

                if (jsonObject.ContainsKey("gameState"))
                {
                    string gameState = jsonObject["gameState"].ToString();
                    if (gameState == "stop")
                    {
                        try
                        {
                            GameController.StopGame();

                        }
                        catch (Exception ex)
                        {

                            Debug.LogError($"Error stopping game: {ex.Message}");
                        }

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

    public async void SendMessageToSocket(string message)
    {
        if (webSocket.State == WebSocketState.Open)
        {
            var bytes = Encoding.UTF8.GetBytes(message);
            await webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
            Debug.Log($"Sent: {message}");
        }
        else
        {
            Debug.LogError("WebSocket is not connected.");
        }
    }

    private void Update()
    {
        // Example: Send a message when the spacebar is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SendMessageToSocket("Hello from Unity!");
            GameOverScreen.Setup();
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