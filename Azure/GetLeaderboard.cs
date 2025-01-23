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
               var items = await GetPlayers();
                _logger.LogInformation("C# HTTP trigger function processed a request.");
                return new OkObjectResult(items);

            }
            catch (System.Exception e)
            {
                _logger.LogError(e, "An error occurred while executing GetLeaderboard");
                return new ObjectResult(new { error = e.Message }) { StatusCode = 500 };
            }

        }


        [Function("AddPerson")]
        public async Task<IActionResult> AddScore([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "leaderboard")] HttpRequest req)
        {

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var player = JsonConvert.DeserializeObject<Player>(requestBody);
                if (player == null)
                {
                    return new BadRequestObjectResult("Invalid person data");
                }
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
        [Function("ResetLeaderboard")]
        public IActionResult ResetLeaderboard([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "leaderboard/reset")] HttpRequest req)
        {
            _leaderboardService.ClearLeaderboard();

            return new OkObjectResult("Leaderboard reset.");
        }

        [Function("TestTrigger")]
        public IActionResult TestTrigger([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "leaderboard/test")] HttpRequest req)
        {
            return new OkObjectResult("test");
        }


        public async Task<List<Player>> GetPlayers()
        {
            string connectionString = "AccountEndpoint=https://entertainendeietsamestorage.documents.azure.com:443/;AccountKey=loFdA92EqB4aMyfHhAlziuCYz814aTP1oNdzwedArUlJzWLiVdVNDpHkIBwyFXVOzuCPdtlEFUeuACDbaQNUEg==;";
            CosmosClient cosmosClient = new CosmosClient(connectionString);
            var container = cosmosClient.GetContainer("FietsGame", "Leaderboard");
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
    }

}

