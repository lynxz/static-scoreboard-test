using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Azure.Data.Tables;
using System.Linq;

namespace StaticScoreboard.GetEntries
{
    public static class GetEntries
    {
        [FunctionName("GetEntries")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
                var tableClient = new TableClient(connectionString, "Scoreboard");
                var entities = tableClient.Query<TableEntity>();
                var items = entities.Select(e => MapTableEntityToScoreModel(e)).OrderByDescending(s => s.Score).Take(100).ToList();

                return new JsonResult(items);
            }
            catch (Exception e)
            {
                log.LogError(e, "Failed to get scoreboard data");
                return new JsonResult(Enumerable.Empty<ScoreModel>().ToList());
            }
        }

        public static ScoreModel MapTableEntityToScoreModel(TableEntity entity)
        {
            var score = new ScoreModel();
            score.Board = entity.PartitionKey;
            score.UserName = entity.RowKey;
            score.Date = entity.Timestamp?.DateTime ?? DateTime.MinValue;
            score.Score = entity.GetInt64("Score") ?? 0L;

            return score;
        }
    }



    public class ScoreModel
    {

        public string Board { get; set; }

        public string UserName { get; set; }

        public long Score { get; set; }

        public DateTime Date { get; set; }

    }
}
