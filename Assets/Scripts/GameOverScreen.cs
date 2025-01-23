using UnityEngine;
using TMPro;
using System.Collections;
using Models.WebSocketMessage;
public class GameOverScreen : MonoBehaviour
{
    public TMP_Text countdown;
    public int countdownTime = 10;
    public GameSelect gameSelect;

    public static bool isGameOver = false;

    public void OnEnable()
    {
        StartCoroutine(CountdownTest());
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



}
