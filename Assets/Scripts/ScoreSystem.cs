using UnityEngine;
using TMPro;
// using UnityEngine.UI;

public class ScoreSystem : MonoBehaviour
{
    float distance_for_1_point = 1f;
    Vector3 previousPlayerPosition;
    float distanceSinceLastScore;

    public Transform player;
    public static float score = 0f;
    public TMP_Text scoreText;


    void Awake()
    {
        //save start pos
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
            WebSocketClient.Instance.SendMessageToSocket($"Score updated: {score}");
        }
    }
}

