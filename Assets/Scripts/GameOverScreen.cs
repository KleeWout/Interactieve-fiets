using UnityEngine;
using TMPro;
using System.Collections;
using Models.WebSocketMessage;
using UnityEngine.Networking;
public class GameOverScreen : MonoBehaviour
{
    public TMP_Text countdown;
    public int countdownTime = 10;
    public GameSelect gameSelect;

    public static bool isGameOver = false;
    private static string url = "https://entertainendefietsgameleaderboard.azurewebsites.net/api/leaderboard?code=Q8dqx9qcNI8yuXDZOCWP05B8pC7fZED6ymj4S5RHVFMPAzFuOZqv8w==";

    public string localUrl = "http://localhost:3000/api/addleaderboard";

    public bool sendLocal = true;



    public void OnEnable()
    {
        GameSelect.isGameStarted = false;
        StartCoroutine(CountdownTest());
        int score = (int)ScoreSystem.score;

        if (sendLocal)
        {
            StartCoroutine(UploadScoreLocal(GameSelect.userName, score));
        }
        else
        {
            StartCoroutine(UploadScore(GameSelect.userName, score));
        }
        StartCoroutine(UploadScore(GameSelect.userName, score));
        StartCoroutine(UploadScoreLocal(GameSelect.userName, score));
        WebSocketClient.Instance.SendMessageToSocket(new WebSocketMessage { GameOver = "true", Score = ScoreSystem.score.ToString() });

    }

    public IEnumerator CountdownTest()
    {
        for (int i = countdownTime; i >= 1; i--)
        {
            countdown.text = $"Terug naar beginscherm in {i} ...";
            yield return new WaitForSeconds(1);
        }
        countdown.text = $"Terug naar beginscherm in {0} ...";

        WebSocketClient.Instance.SendMessageToSocket(new WebSocketMessage { NewConnection = true });
        GameSelect.isGameStarted = false;
        GameSelect.isIdle = false;

        gameObject.SetActive(false);
        countdown.text = $"Terug naar beginscherm in 10...";

    }

    public IEnumerator UploadScore(string name, int score)
    {
        Debug.Log("Sending score from post score");
        string body = "{ \"name\": \"" + name + "\", \"score\": " + score + " }";
        using (UnityWebRequest www = UnityWebRequest.Post(url, body, "application/json"))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
            }
        }
    }

    public IEnumerator UploadScoreLocal(string name, int score)
    {
        string body = "{ \"name\": \"" + name + "\", \"score\": " + score + " }";
        using (UnityWebRequest www = UnityWebRequest.Post(localUrl, body, "application/json"))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
            }
        }
    }



}
