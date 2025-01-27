using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using System.Threading;

public class Leaderboard : MonoBehaviour
{
[SerializeField]
private TMP_Text[] names;

[SerializeField]
private TMP_Text[] scores;

    private string localUrl = "http://localhost:3000/api/addleaderboard";


    public void SetLeaderboard(string name, string score)
    {
        for (int i =0; i < names.Length; i++)
        {
            names[i].text = name;
            scores[i].text = score;
        }
        
    }
}
