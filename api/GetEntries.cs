using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Azure.Data.Tables;
using System.Linq;
using Azure;

namespace api
{
    public static class GetEntries
    {
        [FunctionName("GetEntries")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "getentries/{boardName}")] HttpRequest req,
            string boardName,
            ILogger log)
        {
            log.LogInformation($"Fetching entries for {boardName}");
            try
            {
                var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
                var tableClient = new TableClient(connectionString, "Scoreboard");

                var scoreboardEntity = (await tableClient.GetEntityAsync<ScoreboardTokenEntity>(boardName, "Token")).Value;
                var lowScore = (await tableClient.GetEntityAsync<BoardDataEntity>(boardName, "LowScore")).Value;

                var scores = await tableClient.QueryAsync<ScoreEntity>(s => s.PartitionKey == boardName && s.RowKey != "LowScore" && s.Score >= lowScore.LowScore).ToListAsync();

                return new JsonResult(scores.OrderByDescending(s => s.Score).Take(scoreboardEntity.NumberOfEntries).Select(s => new ScoreResponseDto {
                    Board = s.PartitionKey,
                    UserName = s.UserName,
                    Date = s.Timestamp.Value.DateTime,
                    Score = s.Score
                }).ToList());
            }
            catch (RequestFailedException requestFailedException) {
                log.LogError(requestFailedException, "Failed to get scoreboard data " + requestFailedException.ToString());
                return new JsonResult(Enumerable.Empty<ScoreResponseDto>().ToList());
            }
            catch (Exception e)
            {
                log.LogError(e, "Failed to get scoreboard data");
                return new JsonResult(Enumerable.Empty<ScoreResponseDto>().ToList());
            }
        }
    }
}
