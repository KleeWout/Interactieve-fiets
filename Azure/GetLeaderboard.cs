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
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "leaderboard")] HttpRequest req)
        {
            var leaderboardRepo = new LeaderboardRepository();
            var players = leaderboardRepo.GetPlayers();
            var leaderboard = new Leaderboard { Players = players };

            return new OkObjectResult(leaderboard);
        }


        [Function("AddPerson")]
        public async Task<IActionResult> AddScore([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "leaderboard")] HttpRequest req)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<Player>(requestBody);

            if (data == null || string.IsNullOrEmpty(data.Name) || data.Score <= 0)
            {
                return new BadRequestObjectResult("Invalid player data.");
            }

            var leaderboardRepo = new LeaderboardRepository();
            var response = leaderboardRepo.AddPlayer(data);

            return new OkObjectResult(response);

        }
        [Function("ResetLeaderboard")]
        public IActionResult ResetLeaderboard([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "leaderboard/reset")] HttpRequest req)
        {
            var leaderboardRepo = new LeaderboardRepository();
            leaderboardRepo.ResetLeaderboard();

            return new OkObjectResult("Leaderboard reset.");
        }
    }
}

