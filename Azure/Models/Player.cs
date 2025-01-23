public class Player
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("score")]
    public int Score { get; set; }
}

public class Leaderboard
{
    [JsonProperty("players")]
    public List<Player> Players { get; set; } = new List<Player>();
}
