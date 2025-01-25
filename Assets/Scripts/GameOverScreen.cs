using UnityEngine;
using TMPro;
using System.Collections;
using Models.WebSocketMessage;
using UnityEngine.Networking;
using System.Threading;
public class GameOverScreen : MonoBehaviour
{
    public TMP_Text countdown;
    public TMP_Text scoreEndText;
    public int countdownTime = 10;
    public GameSelect gameSelect;
    public GameObject chaser;
    public ScoreSystem scoreSystem;
    public static bool isGameOver = false;
    private static string url = "https://entertainendefietsgameleaderboard.azurewebsites.net/api/leaderboard?code=Q8dqx9qcNI8yuXDZOCWP05B8pC7fZED6ymj4S5RHVFMPAzFuOZqv8w==";

    private string localUrl = "http://localhost:3000/api/addleaderboard";

    public bool sendLocal = true;

    private CancellationTokenSource cts;
    public TerrainGen terrain;



    public void OnEnable()
    {
        GameSelect.isGameStarted = false;
        int score = (int)ScoreSystem.score;
        scoreEndText.text = score.ToString() + "m";
        chaser.SetActive(false);
        WebSocketClient.Instance.SendMessageToSocket(new WebSocketMessage { GameOver = "true", Score = score.ToString() });
        StartCoroutine(CountdownTest());


        if (sendLocal)
        {
            StartCoroutine(UploadScoreLocal(GameSelect.userName, score));
        }
        else
        {
            StartCoroutine(UploadScore(GameSelect.userName, score));
        }
    }

    public IEnumerator CountdownTest()
    {
        gameSelect.ResetPlayer();
        gameSelect.StartGame();
        GameSelect.isGameStarted = false;

        cts?.Cancel();
        cts = new CancellationTokenSource();
        terrain.GenerateTerrain(GameSelect.gameMode, cts.Token);

        for (int i = countdownTime; i >= 1; i--)
        {
            countdown.text = $"Terug naar beginscherm in {i} ...";
            yield return new WaitForSeconds(1);
        }
        countdown.text = $"Terug naar beginscherm in {0} ...";

        // WebSocketClient.Instance.SendMessageToSocket(new WebSocketMessage { NewConnection = true });
        // GameSelect.isGameStarted = false;
        // GameSelect.isIdle = false;

        gameObject.SetActive(false);
        GameSelect.isGameStarted = true;
        WebSocketClient.Instance.SendMessageToSocket(new WebSocketMessage { GameOver = "restart", Score = "0" });
        countdown.text = $"Terug naar beginscherm in 10...";

    }

    public IEnumerator UploadScore(string name, int score)
    {
        Debug.Log("Sending score from post score");
        string body = "{ \"name\": \"" + name + "\", \"score\": " + score + " }";
        using (UnityWebRequest www = UnityWebRequest.Post(url, body, "application/json"))
        {
            yield return www.SendWebRequest();
        }
    }

    public IEnumerator UploadScoreLocal(string name, int score)
    {
        string body = "{ \"name\": \"" + name + "\", \"score\": " + score + " }";
        using (UnityWebRequest www = UnityWebRequest.Post(localUrl, body, "application/json"))
        {
            yield return www.SendWebRequest();
        }
    }



}
