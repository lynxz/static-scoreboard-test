using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;

namespace StaticScoreboard.GetEntries
{
    public static class GetEntries
    {
        [FunctionName("GetEntries")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            [Table("Scoreboard", Connection = "AzureWebJobsStorage")] CloudTable cloudTable,
            ILogger log)
        {
            var query = new TableQuery<ScoreEntity>().Take(100);
            var items = new List<ScoreEntity>();
            TableContinuationToken token = null;

            do
            {
                var seg = await cloudTable.ExecuteQuerySegmentedAsync(query, token);
                token = seg.ContinuationToken;
                items.AddRange(seg.Results);
            } while (token != null);

            return new JsonResult(items);
        }
    }

    public class ScoreEntity : TableEntity
    {

        public ScoreEntity(string game, string userName)
        {
            PartitionKey = game;
            RowKey = userName;
        }

        public ScoreEntity() { }

        public long Score { get; set; }

        public DateTime Date { get; set; }

    }
}
