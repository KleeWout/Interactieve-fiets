using UnityEngine;
using TMPro;
public class GameOverScreen : MonoBehaviour
{
    public TMP_Text score;

    public void Setup()
    {
        gameObject.SetActive(true);
        WaterBob.isSunk = true;
        score.text = "behaalde score: " + ScoreSystem.score.ToString() + "m";
        Debug.Log("Game Over");
    }

}
