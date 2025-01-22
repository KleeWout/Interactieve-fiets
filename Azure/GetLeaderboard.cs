using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using FietsGame.Repository;
using System.Text.Json.Nodes;

namespace FietsGame.Function
{
    public class GetLeaderboard
    {
        private readonly ILogger<GetLeaderboard> _logger;

        public GetLeaderboard(ILogger<GetLeaderboard> logger)
        {
            _logger = logger;
        }

        [Function("GetLeaderboard")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
        {
            var leaderboardRepo = new LeaderboardRepo();
            var players = leaderboardRepo.GetPersons();
            var leaderboard = new Leaderboard { Players = players };

            return new OkObjectResult(leaderboard);
        }


        [Function("AddPerson")]
        public async Task<IActionResult> AddScore([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "update")] HttpRequest req)
        {

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<Player>(requestBody);

            if (data == null || string.IsNullOrEmpty(data.Name) || data.Score <= 0)
            {
                return new BadRequestObjectResult("Invalid player data.");
            }

            var filename = "c:/Users/woutk/Documents/GitHub/Interactieve-fiets/Azure/data/leaderBoard.json";
            var json = File.ReadAllText(filename);
            var leaderboard = JsonConvert.DeserializeObject<Leaderboard>(json);

            var existingPlayer = leaderboard.Players.FirstOrDefault(p => p.Name == data.Name);
            if (existingPlayer != null)
            {
                existingPlayer.Score = data.Score;
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
            File.WriteAllText(filename, updatedJson);

            return new OkObjectResult(updatedJson);
        }
    }
}

