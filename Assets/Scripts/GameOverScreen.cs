using UnityEngine;
using TMPro;
public class GameOverScreen : MonoBehaviour
{
    public TMP_Text score;

    public static bool isGameOver = false;

    public void Setup()
    {
        Debug.Log("Calling GameOver");
        gameObject.SetActive(true);
        WaterBob.isSunk = true;
        score.text = "behaalde score: " + ScoreSystem.score.ToString() + "m";
        // WebSocketClient.Instance.SendMessageToSocket($"{{\"score\" : \"{ScoreSystem.score}\"}}");
        Debug.Log("Game Over");
    }



}
