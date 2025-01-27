using Newtonsoft.Json;
using System.Collections.Generic;

namespace Models.Leaderboard{
public class Player
{
    [JsonProperty("position")]
    public int Position { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("score")]
    public int Score { get; set; }
}

// public class Leaderboard
// {
//     [JsonProperty("players")]
//     public List<Player> Players { get; set; } = new List<Player>();
// }

}
