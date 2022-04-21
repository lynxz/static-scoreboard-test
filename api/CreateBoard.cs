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
    public static class CreateBoard
    {
        [FunctionName("CreateBoard")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "createboard/{boardName}")] HttpRequest req,
            string boardName,
            ILogger log)
        {
            try
            {
                var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
                var tableClient = new TableClient(connectionString, "Scoreboard");

                var token = Guid.NewGuid().ToString();
                try
                {
                    await tableClient.AddEntityAsync(new ScoreboardTokenEntity
                    {
                        PartitionKey = boardName,
                        Token = token
                    });
                    await tableClient.AddEntityAsync(new LowScoreEntity {
                        PartitionKey = boardName,
                        RowKey = "LowScore",
                        Score = -1
                    });
                }
                catch (Exception e)
                {
                    return new JsonResult(e);
                }

                return new JsonResult(new { Name = boardName, Token = token });
            }
            catch (Exception e)
            {
                log.LogError(e, "Failed to get scoreboard data");
                return new JsonResult(Enumerable.Empty<ScoreModel>().ToList());
            }
        }
    }

}
