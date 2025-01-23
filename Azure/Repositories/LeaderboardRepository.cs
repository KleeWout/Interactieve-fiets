using System.Configuration;

namespace FietsGame.Repositories;

public interface ILeaderboardRepository
{
    Task<List<Player>> GetPlayers();

    Task<Player> AddPlayer(Player player);

    Task ClearLeaderboard();
}

public class LeaderboardRepository : ILeaderboardRepository
{
    private readonly CosmosClient _cosmosClient;



    public LeaderboardRepository()
    {

        //add var to the variables inside the azure portal
        var cosmosDbConnectionString = Environment.GetEnvironmentVariable("CosmosDb");
        if (string.IsNullOrEmpty(cosmosDbConnectionString))
        {
            throw new InvalidOperationException("CosmosDb connection string is not set in environment variables.");
        }
        _cosmosClient = new CosmosClient(cosmosDbConnectionString);
    }

    public async Task<List<Player>> GetPlayers()
    {
        var container = _cosmosClient.GetContainer("FietsGame", "Leaderboard");
        var query = new QueryDefinition("SELECT * FROM c ORDER BY c.score DESC");
        var iterator = container.GetItemQueryIterator<Player>(query);

        List<Player> results = new List<Player>();
        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync();
            results.AddRange(response.ToList());
        }
        return results;
    }

    public async Task<Player> AddPlayer(Player newPlayer)
    {
        var container = _cosmosClient.GetContainer("FietsGame", "Leaderboard");

        try
        {
            // Check if player exists
            var query = new QueryDefinition("SELECT * FROM c WHERE c.name = @name")
                .WithParameter("@name", newPlayer.Name);
            var iterator = container.GetItemQueryIterator<Player>(query);
            var existingPlayer = (await iterator.ReadNextAsync()).FirstOrDefault();

            if (existingPlayer != null)
            {
                Console.WriteLine($"Player {existingPlayer.Name} already exists with score {existingPlayer.Score}");
                // Update score if new score is higher
                if (newPlayer.Score > existingPlayer.Score)
                {
                    existingPlayer.Score = newPlayer.Score;
                    await container.ReplaceItemAsync(existingPlayer, existingPlayer.Id,
                        new PartitionKey(existingPlayer.Name));
                }
                return existingPlayer;
            }
            else
            {
                // Create new player using Name as both ID and partition key
                newPlayer.Id = Guid.NewGuid().ToString();
                await container.CreateItemAsync(newPlayer, new PartitionKey(newPlayer.Name));
                return newPlayer;
            }
        }
        catch (CosmosException ex)
        {
            throw new Exception($"Error accessing database: {ex.Message}", ex);
        }
    }

    public async Task ClearLeaderboard()
    {
        var container = _cosmosClient.GetContainer("FietsGame", "Leaderboard");
        var query = new QueryDefinition("SELECT * FROM c");
        var iterator = container.GetItemQueryIterator<Player>(query);

        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync();
            foreach (var player in response)
            {
                await container.DeleteItemAsync<Player>(player.Id, new PartitionKey(player.Name));
            }
        }
    }
}
