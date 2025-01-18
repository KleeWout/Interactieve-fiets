#nullable enable

namespace Models.WebSocketMessage
{
    public class WebSocketMessage
    {
        public bool IsGameClient = true;
        public string? GameState { get; set;}



        // public string? Secret { get; set;}

        // public WebSocketMessage()
        // {
        //     Secret = "your_secret_value"; // Set your secret value here
        // }

        // public override string ToString()
        // {
        //     return $"GameState: {GameState}, Code: {Code}, Score: {Score}";
        // }
    }
}