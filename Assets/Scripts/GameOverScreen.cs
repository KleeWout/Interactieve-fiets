using UnityEngine;
using TMPro;
using System.Collections;
using Models.WebSocketMessage;
using UnityEngine.Networking;
using System.Threading;
using Newtonsoft.Json;
using Models.Leaderboard;
using System.Collections.Generic;
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
    private string getUrl = "http://localhost:3000/api/getleaderboard";

    public bool sendLocal = true;

    private CancellationTokenSource cts;
    public TerrainGen terrain;
    private Coroutine uploadScoreCoroutine;
    private Coroutine countdownCoroutine;

    // public TMP_Text[] endLeaderboard;

    [SerializeField]
    private TMP_Text[] names;

    [SerializeField]
    private TMP_Text[] scores;


    public void OnEnable()
    {
        GameSelect.isGameStarted = false;
        int score = (int)ScoreSystem.score;
        scoreEndText.text = score.ToString() + "m";
        chaser.SetActive(false);

        if (uploadScoreCoroutine != null)
        {
            StopCoroutine(uploadScoreCoroutine);
        }

        if (sendLocal)
        {
            uploadScoreCoroutine = StartCoroutine(UploadScoreLocal(GameSelect.userName, score));
        }
        else
        {
            uploadScoreCoroutine = StartCoroutine(UploadScore(GameSelect.userName, score));
        }

        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
        }
        countdownCoroutine = StartCoroutine(CountdownTest());

        if(GameSelect.gameMode == Models.GameModeModel.GameMode.SinglePlayer){
            gameSelect.LoadSinglePlayer();
        }
        else if(GameSelect.gameMode == Models.GameModeModel.GameMode.MultiPlayer){
            gameSelect.LoadMultiPlayer();
        }
        StartCoroutine(GetLocalScore(getUrl));

        StartCoroutine(CountdownTest());
    }

    public IEnumerator CountdownTest()
    {
        gameSelect.ResetPlayer();
        gameSelect.StartGame();
        GameSelect.isGameStarted = false;

        for (int i = countdownTime; i >= 1; i--)
        {
            countdown.text = $"Terug naar beginscherm in {i} ...";
            yield return new WaitForSeconds(1);
        }
        countdown.text = $"Terug naar beginscherm in {0} ...";

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
        WebSocketClient.Instance.SendMessageToSocket(new WebSocketMessage { GameOver = "true", Score = score.ToString() });
    }

    IEnumerator GetLocalScore(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    // Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                    List<Player> players = JsonConvert.DeserializeObject<List<Player>>(webRequest.downloadHandler.text);
                    SetLeaderboard(players);
                    break;
            }

        }
    }
    private void SetLeaderboard(List<Player> players)
    {
        for (int i = 0; i < names.Length; i++)
        {
            names[i].text = players[i].Name;
            scores[i].text = players[i].Score.ToString() + "m";
        }
    }

}
