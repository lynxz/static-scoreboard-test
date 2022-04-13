using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Data.Tables;
using Azure;
using System.Linq;

namespace api
{
    public static class UploadScore
    {
        [FunctionName("UploadScore")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "uploadScore/{boardName:alpha}")] HttpRequest req,
            string boardName,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<ScoreDto>(requestBody);

            var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
            var tableClient = new TableClient(connectionString, "Scoreboard");

            var scoreboardTokenResponse = await tableClient.GetEntityAsync<ScoreboardTokenEntity>(boardName, "Token");
            if (scoreboardTokenResponse.Value.Token != data.Token) {
                return new BadRequestObjectResult("Invalid token");
            }

            var scoreEntity = new ScoreEntity
            {
                Score = data.Score,
                UserName = data.UserName,
                PartitionKey = boardName,
                RowKey = Guid.NewGuid().ToString()
            };

            await tableClient.AddEntityAsync(scoreEntity);

            LowScoreEntity lowScore = null;
            try
            {
                lowScore = (await tableClient.GetEntityAsync<LowScoreEntity>(boardName, "LowScore")).Value;
                Console.WriteLine("Success");
            }
            catch (RequestFailedException ex)
            {
                if (ex.Status == 404)
                {
                    lowScore = new LowScoreEntity { PartitionKey = boardName, RowKey = "LowScore", Score = data.Score };
                    await tableClient.AddEntityAsync(lowScore);
                }
                else
                {
                    throw;
                }
            }
            if (lowScore.Score < data.Score)
            {
                var entries = await tableClient.QueryAsync<ScoreEntity>(c => c.PartitionKey == boardName && c.RowKey != "LowScore" && c.Score >= lowScore.Score).ToListAsync();
                var lowest = entries.OrderByDescending(s => s.Score).Take(100).Last();
                lowScore.Score = lowest.Score;
                await tableClient.UpdateEntityAsync(lowScore, ETag.All);
            }

            return new OkResult();
        }
    }


    public class LowScoreEntity : ITableEntity
    {
        public long Score { get; set; }
        public int Count { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }

    public class ScoreDto
    {

        public string UserName { get; set; }

        public long Score { get; set; }

        public string Token { get; set; }

    }
}