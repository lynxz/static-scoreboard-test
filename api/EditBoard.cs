using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Azure.Data.Tables;
using System.IO;
using Newtonsoft.Json;

namespace api
{
    public static class EditBoard
    {
        [FunctionName("EditBoard")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "editboard/{boardName}/{token}")] HttpRequest req,
            string boardName,
            string token,
            ILogger log)
        {
            log.LogInformation($"Editing {boardName} and scores");
            try
            {
                var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
                var tableClient = new TableClient(connectionString, "Scoreboard");

                var scoreboardEntity = (await tableClient.GetEntityAsync<ScoreboardTokenEntity>(boardName, "Token")).Value;
                if (scoreboardEntity.Token != token) 
                    return new BadRequestObjectResult("Incorrect Token");

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var updateBoard = JsonConvert.DeserializeObject<UpdateBoardDto>(requestBody);

                if (updateBoard.NumberOfEntries < 1)
                    return new BadRequestObjectResult("Number of entries must be at least 1");
                
                scoreboardEntity.NumberOfEntries = updateBoard.NumberOfEntries;

                var response = tableClient.UpdateEntity(scoreboardEntity, Azure.ETag.All);
                if (response.IsError)
                    return new StatusCodeResult(response.Status);

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