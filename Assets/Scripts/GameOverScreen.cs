using UnityEngine;
using TMPro;
using UnityEngine.Analytics;
public class GameOverScreen : MonoBehaviour
{
    public static TMP_Text score;
    public static GameObject gameOverObject;
    public GameOverScreen gameOverScreen;

    public static bool isGameOver = false;

    public void Awake()
    {
        score = GameObject.Find("Score").GetComponent<TMP_Text>();
        gameOverObject = GameObject.FindWithTag("GameOver");
        // gameOverObject.SetActive(false);
    }

    public static void Setup()
    {
        gameOverObject = GameObject.FindWithTag("GameOver");
        Debug.Log("Calling GameOver");
        gameOverObject.SetActive(true);
        WaterBob.isSunk = true;
        score.text = "behaalde score: " + ScoreSystem.score.ToString() + "m";
        WebSocketClient.Instance.SendMessageToSocket($"{{\"score\" : \"{ScoreSystem.score}\"}}");
        Debug.Log("Game Over");
    }



}
