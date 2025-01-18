using UnityEngine;
using TMPro;
using Models.WebSocketMessage;

public class ScoreSystem : MonoBehaviour
{
    private float distance_for_1_point = 1f;
    private Vector3 previousPlayerPosition;
    private float distanceSinceLastScore;

    private Transform player;
    public static float score = 0f;
    public TMP_Text scoreText;
    private WebSocketClient webSocketClient;

    void Start(){
        webSocketClient = WebSocketClient.Instance;
        player = GetComponent<Transform>();
        previousPlayerPosition = player.position;
    }

    void Update()
    {
        //Calculate dist
        distanceSinceLastScore += player.position.z - previousPlayerPosition.z;
        previousPlayerPosition = player.position;

        int scoreToAdd = 0;
        //minimum afstand vooraleer score bijtelt
        while (distanceSinceLastScore > distance_for_1_point)
        {
            ++scoreToAdd;
            distanceSinceLastScore -= distance_for_1_point;
        }

        if (scoreToAdd > 0)
        {
            score += scoreToAdd;
            scoreText.text = score.ToString() + "m";
            webSocketClient.SendMessageToSocket(new WebSocketMessage { Score = score.ToString() });
        }
    }
}

