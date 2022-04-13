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

namespace funcs
{
    public static class UploadScore
    {
        [FunctionName("UploadScore")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<ScoreDto>(requestBody);

            var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
            var tableClient = new TableClient(connectionString, "Scoreboard");

            var lowScore = (await tableClient.GetEntityAsync<TableEntity>("LowScore", "Delimiter")).Value;

            if (lowScore.GetInt64("Score") < data.Score) {
                tableClient.QueryAsync<TableEntity>($"PartitionKey eq HighScore and Score ge {lowScore.GetInt64("Score")}");
                
            }

            return new OkResult();
        }
    }

    public class LowScore : ITableEntity
    {
        public long Score { get; set; }
        public int Count { get; set; }
        public string PartitionKey { get;set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }

    public class ScoreDto
    {

        public string UserName { get; set; }

        public long Score { get; set; }

    }
}
