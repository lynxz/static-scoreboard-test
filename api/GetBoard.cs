using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Azure.Data.Tables;

namespace api
{
    public static class GetBoard
    {
        [FunctionName("GetBoard")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "getboard/{boardName}/{token}")] HttpRequest req,
            string boardName,
            string token,
            ILogger log)
        {
            log.LogInformation($"Getting {boardName} data");
            try
            {
                var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
                var tableClient = new TableClient(connectionString, "Scoreboard");

                var scoreboardEntity = (await tableClient.GetEntityAsync<ScoreboardTokenEntity>(boardName, "Token")).Value;
                if (scoreboardEntity.Token != token) {
                    return new BadRequestObjectResult("Incorrect Token");
                }
                
                var scoreboard = new ScoreboardDto {
                    Name = scoreboardEntity.PartitionKey,
                    Email = scoreboardEntity.Email,
                    Token = Guid.Parse(scoreboardEntity.Token),
                    NumberOfEntries = scoreboardEntity.NumberOfEntries
                };

                return new OkObjectResult(scoreboard);
            }
            catch (Exception e)
            {
                log.LogError(e, "Failed to get scoreboard data");
                return new StatusCodeResult(500);
            }
        }
    }
}