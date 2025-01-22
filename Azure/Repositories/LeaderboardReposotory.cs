namespace FietsGame.Repository;


public class LeaderboardRepository : IFileRepository
{
    private const string fileName = "c:/Users/woutk/Documents/GitHub/Interactieve-fiets/Azure/data/leaderBoard.json";

    public List<Player> GetPlayers()
    {
        var json = File.ReadAllText(fileName);
        var leaderboard = JsonConvert.DeserializeObject<Leaderboard>(json);
        return leaderboard?.Players ?? new List<Player>();
    }

    public string AddPlayer(Player data)
    {
        var json = File.ReadAllText(fileName);
        var leaderboard = JsonConvert.DeserializeObject<Leaderboard>(json);

        var existingPlayer = leaderboard.Players.FirstOrDefault(p => p.Name == data.Name);
        if (existingPlayer != null)
        {
            if (data.Score > existingPlayer.Score)
            {
                existingPlayer.Score = data.Score;
            }
        }
        else
        {
            leaderboard.Players.Add(new Player { Name = data.Name, Score = data.Score });
        }

        leaderboard.Players.Sort((a, b) => b.Score - a.Score);
        for (int i = 0; i < leaderboard.Players.Count; i++)
        {
            leaderboard.Players[i].Position = i + 1;
        }

        string updatedJson = JsonConvert.SerializeObject(leaderboard, Formatting.Indented);
        File.WriteAllText(fileName, updatedJson);

        return updatedJson;
    }

    public void ResetLeaderboard()
    {
        var leaderboard = new Leaderboard();
        string updatedJson = JsonConvert.SerializeObject(leaderboard, Formatting.Indented);
        File.WriteAllText(fileName, updatedJson);
    }





}
