using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Azure.Data.Tables;
using System.Linq;

namespace api
{
    public static class DeleteBoard
    {
        [FunctionName("DeleteBoard")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "deleteboard/{boardName}/{token}")] HttpRequest req,
            string boardName,
            string token,
            ILogger log)
        {
            log.LogInformation($"Deleting {boardName} and scores");
            try
            {
                var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
                var tableClient = new TableClient(connectionString, "Scoreboard");

                var scoreboardEntity = (await tableClient.GetEntityAsync<ScoreboardTokenEntity>(boardName, "Token")).Value;
                if (scoreboardEntity.Token != token) {
                    return new BadRequestObjectResult("Incorrect Token");
                }
                
                var scores = tableClient.QueryAsync<ScoreEntity>(s => s.PartitionKey == boardName);
                await foreach(var score in scores) {
                    await tableClient.DeleteEntityAsync(score.PartitionKey, score.RowKey);
                }

                return new OkResult();
            }
            catch (Exception e)
            {
                log.LogError(e, "Failed to get scoreboard data");
                return new StatusCodeResult(500);
            }
        }
    }
}