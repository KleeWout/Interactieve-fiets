using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class WebSocketClient : MonoBehaviour
{
    public static WebSocketClient Instance { get; private set; }

    private ClientWebSocket webSocket;

    private Queue<string> sceneLoadRequests = new Queue<string>();
    private bool sceneChangeRequested = false;

    //keep script active
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

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

                //switch scenes based on message
                if (message == "singleplayer")
                {
                    Debug.Log("Queueing scene change: SinglePlayer");
                    sceneLoadRequests.Enqueue("SinglePlayer");
                    sceneChangeRequested = true;
                }
                else if (message == "multiplayer")
                {
                    Debug.Log("Queueing scene change: MultiPlayer");
                    sceneLoadRequests.Enqueue("MultiPlayer");
                    sceneChangeRequested = true;
                }
                else if (message == "main")
                {
                    Debug.Log("Queueing scene change: Main");
                    sceneLoadRequests.Enqueue("Main");
                    sceneChangeRequested = true;
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
            SendMessageToSocket("Hello from Unity!");
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