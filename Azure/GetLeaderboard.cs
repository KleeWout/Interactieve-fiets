using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json.Nodes;

namespace FietsGame.Function
{
    public class GetLeaderboard
    {
        private readonly ILogger<GetLeaderboard> _logger;
        private readonly ILeaderboardService _leaderboardService;

        public GetLeaderboard(ILogger<GetLeaderboard> logger, ILeaderboardService leaderboardService)
        {
            _logger = logger;
            _leaderboardService = leaderboardService;
        }



        public GetLeaderboard(ILogger<GetLeaderboard> logger)
        {
            _logger = logger;
        }

        [Function("GetLeaderboard")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "leaderboard")] HttpRequest req)
        {
            try
            {
                var players = await _leaderboardService.GetPlayers();
                return new OkObjectResult(players);
                // var leaderboardRepo = new LeaderboardRepository();
                // var players = leaderboardRepo.GetPlayers();
                // var leaderboard = new Leaderboard { Players = players };

                // return new OkObjectResult(leaderboard);
            }
            catch (System.Exception e)
            {
                return new OkObjectResult(e.Message);
                throw;
            }

        }


        [Function("AddPerson")]
        public async Task<IActionResult> AddScore([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "leaderboard")] HttpRequest req)
        {

            try
            {
                // Read the entire request body as a string
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                // Deserialize the JSON string into a Person object
                var player = JsonConvert.DeserializeObject<Player>(requestBody);

                // Check if deserialization was successful
                if (player == null)
                {
                    return new BadRequestObjectResult("Invalid person data");
                }

                // Call the service to add the person to the database
                var addedPlayer = await _leaderboardService.AddPlayer(player);
                return new OkObjectResult(addedPlayer);
            }
            catch (JsonReaderException ex)
            {
                _logger.LogError(ex, "Error deserializing person data");
                return new BadRequestObjectResult("Invalid JSON format");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddPerson");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }



        //         string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        //         var data = JsonConvert.DeserializeObject<Player>(requestBody);

        //             if (data == null || string.IsNullOrEmpty(data.Name) || data.Score <= 0)
        //             {
        //                 return new BadRequestObjectResult("Invalid player data.");
        //             }

        //             var leaderboardRepo = new LeaderboardRepository();
        //         var response = leaderboardRepo.AddPlayer(data);

        //             return new OkObjectResult(response);

        //     }
        [Function("ResetLeaderboard")]
        public IActionResult ResetLeaderboard([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "leaderboard/reset")] HttpRequest req)
        {
             _leaderboardService.ClearLeaderboard();

            return new OkObjectResult("Leaderboard reset.");
        }
    }
}

