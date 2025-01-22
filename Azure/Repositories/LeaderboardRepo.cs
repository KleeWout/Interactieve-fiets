namespace FietsGame.Repository;

public class LeaderboardRepo : IFileRepository
{
    private const string fileName = "c:/Users/woutk/Documents/GitHub/Interactieve-fiets/Azure/data/leaderBoard.json";

        public List<Player> GetPersons()
    {
        var json = File.ReadAllText(fileName);
        var leaderboard = JsonConvert.DeserializeObject<Leaderboard>(json);
        return leaderboard?.Players ?? new List<Player>();
    }
}
