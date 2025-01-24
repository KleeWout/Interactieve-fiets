using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PostScore : MonoBehaviour
{

    private static string url = "https://entertainendefietsgameleaderboard.azurewebsites.net/api/leaderboard?code=Q8dqx9qcNI8yuXDZOCWP05B8pC7fZED6ymj4S5RHVFMPAzFuOZqv8w==";


    public IEnumerator Upload(string name, int score)
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
}