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
    public static class GetEntries
    {
        [FunctionName("GetEntries")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "getentries/{boardName:alpha}")] HttpRequest req,
            string boardName,
            ILogger log)
        {
            try
            {
                var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
                var tableClient = new TableClient(connectionString, "Scoreboard");

                var lowScore = (await tableClient.GetEntityAsync<LowScoreEntity>(boardName, "LowScore")).Value;

                var scores = await tableClient.QueryAsync<ScoreEntity>(s => s.PartitionKey == boardName && s.RowKey != "LowScore" && s.Score >= lowScore.Score).ToListAsync();

                return new JsonResult(scores.OrderByDescending(s => s.Score).Take(100).Select(s => new ScoreModel {
                    Board = s.PartitionKey,
                    UserName = s.UserName,
                    Date = s.Timestamp.Value.DateTime,
                    Score = s.Score
                }).ToList());
            }
            catch (Exception e)
            {
                log.LogError(e, "Failed to get scoreboard data");
                return new JsonResult(Enumerable.Empty<ScoreModel>().ToList());
            }
        }
    }
}
