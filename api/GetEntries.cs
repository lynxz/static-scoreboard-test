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
using Scoreboard.Api.Response;

namespace Scoreboard.Api
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

                var scoreboardNameEntity = (await tableClient.GetEntityAsync<BoardNameEntity>(boardName, "Name")).Value;
                var scoreboardEntity = (await tableClient.GetEntityAsync<BoardDataEntity>(scoreboardNameEntity.Token, "Data")).Value;

                var scoreClient = new TableClient(connectionString, boardName);
                var lowScore = (await scoreClient.GetEntityAsync<BoardScoreCounterEntity>(boardName, "LowScore")).Value;

                var scores = await scoreClient.QueryAsync<ScoreEntity>(s => s.PartitionKey == boardName && s.RowKey != "LowScore" && s.Score >= lowScore.LowScore).ToListAsync();

                return new JsonResult(scores.OrderByDescending(s => s.Score).Take(scoreboardEntity.NumberOfEntries).Select(s => new ScoreDto {
                    Board = s.PartitionKey,
                    UserName = s.UserName,
                    Date = s.Timestamp.Value.DateTime,
                    Score = s.Score
                }).ToList());
            }
            catch (RequestFailedException requestFailedException) {
                log.LogError(requestFailedException, "Failed to get scoreboard data " + requestFailedException.ToString());
                return new JsonResult(Enumerable.Empty<ScoreDto>().ToList());
            }
            catch (Exception e)
            {
                log.LogError(e, "Failed to get scoreboard data");
                return new JsonResult(Enumerable.Empty<ScoreDto>().ToList());
            }
        }
    }
}
