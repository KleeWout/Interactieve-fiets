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
using System.Data.Common;
using TMPro;



public class WebSocketClient : MonoBehaviour
{
    public static WebSocketClient Instance;
    public string serverAddress = "localhost:8080";


    private ClientWebSocket webSocket = new ClientWebSocket();

    private Queue<string> sceneLoadRequests = new Queue<string>();
    private bool sceneChangeRequested = false;

    private GameSelect gameSelect;

    public TMP_Text username;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private async void Start()
    {
        gameSelect = GetComponent<GameSelect>();
        Uri serverUri = new Uri($"ws://{serverAddress}");

        Debug.Log("Connecting to WebSocket server...");
        await webSocket.ConnectAsync(serverUri, CancellationToken.None);
        Debug.Log("Connected to WebSocket server!");

        // Start receiving messages
        ReceiveMessages();
        SendMessageToSocket(new WebSocketMessage { NewConnection = true });
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
                if (jsonObject.ContainsKey("connectionStatus"))
                {
                    string connectionStatus = jsonObject["connectionStatus"].ToString();
                    if (connectionStatus == "connected" && !GameSelect.isGameStarted)
                    {
                        if (jsonObject.ContainsKey("gameMode") && jsonObject["gameMode"].ToString() == "multiplayer")
                        {
                            gameSelect.ChangeState(GameMode.MultiPlayer);
                        }
                        else if(jsonObject.ContainsKey("gameMode") && jsonObject["gameMode"].ToString() == "singleplayer")
                        { 
                            gameSelect.ChangeState(GameMode.SinglePlayer);
                        }
                    }
                    else if(connectionStatus == "connected" && GameSelect.isGameStarted){
                        GameSelect.isIdle = false;
                    }
                    else if (connectionStatus == "disconnected")
                    {
                        gameSelect.ChangeState(GameMode.Menu);
                    }
                    else if(connectionStatus == "idle"){
                        GameSelect.isIdle = true;
                    }
                }
                if (jsonObject.ContainsKey("userName"))
                {
                    string userName = jsonObject["userName"].ToString();
                    GameSelect.userName = userName;
                    username.text = "Proficiat, " + userName;
                }
                if (jsonObject.ContainsKey("gameMode"))
                {
                    string gameMode = jsonObject["gameMode"].ToString();
                    if (gameMode == "singleplayer")
                    {
                        gameSelect.SwitchGameMode(GameMode.SinglePlayer);
                    }
                    else if (gameMode == "multiplayer")
                    {
                        gameSelect.SwitchGameMode(GameMode.MultiPlayer);
                    }
                }
                if (jsonObject.ContainsKey("gameState"))
                {
                    string gameState = jsonObject["gameState"].ToString();
                    if (gameState == "start")
                    {
                        gameSelect.StartGame();
                        // if(jsonObject["gameMode"].ToString() == "multiplayer"){
                        //     gameSelect.StartGame(GameMode.MultiPlayer);
                        // }
                        // else if(jsonObject["gameMode"].ToString() == "singleplayer"){
                        //     gameSelect.StartGame(GameMode.SinglePlayer);
                        // }
                    }
                    else if (gameState == "stop")
                    {
                        Debug.Log("stop");
                    }
                    else if (gameState == "restart")
                    {
                        Debug.Log("restart");
                    }

                }

            }
        }
        catch (Exception)
        {
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
            SceneManager.LoadScene(sceneName);
            sceneChangeRequested = false;
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