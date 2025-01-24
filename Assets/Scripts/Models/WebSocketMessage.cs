#nullable enable

namespace Models.WebSocketMessage
{
    [System.Serializable]
    public class WebSocketMessage
    {
        public bool IsGameClient = true;
        public bool NewConnection = false;
        public string? Score;
        public string? GameOver;

    }
}