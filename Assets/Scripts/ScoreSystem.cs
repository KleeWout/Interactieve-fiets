using UnityEngine;
using TMPro;
using Models.WebSocketMessage;

public class ScoreSystem : MonoBehaviour
{

    private Transform player;
    public static int score = 0;
    private int lastScore = 0;
    public TMP_Text scoreText;
    // public TMP_Text scoreEndText;
    private WebSocketClient webSocketClient;

    void Start(){
        webSocketClient = WebSocketClient.Instance;
        player = GetComponent<Transform>();
    }

    void Update()
    {
        // //Calculate dist
        // distanceSinceLastScore += player.position.z - previousPlayerPosition.z;
        // previousPlayerPosition = player.position;

        // int scoreToAdd = 0;
        // //minimum afstand vooraleer score bijtelt
        // while (distanceSinceLastScore > distance_for_1_point)
        // {
        //     ++scoreToAdd;
        //     distanceSinceLastScore -= distance_for_1_point;
        // }

        // // Debug.Log()

        // if (scoreToAdd > 0 && GameSelect.isGameStarted)
        // {
        //     score += scoreToAdd;
        //     scoreText.text = score.ToString() + "m";
        //     scoreEndText.text = score.ToString() + "m";
        //     webSocketClient.SendMessageToSocket(new WebSocketMessage { Score = score.ToString() });
        // }

        
        if (lastScore < (int)Mathf.Round(player.position.z) && GameSelect.isGameStarted)
        {
            score = (int)Mathf.Round(player.position.z);
            scoreText.text = score.ToString() + "m";
            // scoreEndText.text = score.ToString() + "m";
            webSocketClient.SendMessageToSocket(new WebSocketMessage { Score = score.ToString() });
            lastScore = score;
        }
    }

    public void ResetScore()
    {
        score = 0;
        lastScore = 0;
        scoreText.text = score.ToString() + "m";
        // scoreEndText.text = score.ToString() + "m";
    }


}

