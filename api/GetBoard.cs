using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Azure.Data.Tables;
using Scoreboard.Api.Response;
using Azure;

namespace Scoreboard.Api
{
    public static class GetBoard
    {
        [FunctionName("GetBoard")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "getboard/{token}")] HttpRequest req,
            string boardName,
            string token,
            ILogger log)
        {
            log.LogInformation($"Getting {boardName} data");
            try
            {
                var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
                var tableClient = new TableClient(connectionString, "Scoreboard");

                BoardDataEntity scoreboardEntity = null;
                try
                {
                    scoreboardEntity = (await tableClient.GetEntityAsync<BoardDataEntity>(token, "Data")).Value;
                }
                catch (RequestFailedException ex)
                {
                    if (ex.Status == 404)
                        return new BadRequestObjectResult("Incorrect Token");

                    throw;
                }

                var scoreboard = new ScoreboardDto
                {
                    Name = scoreboardEntity.Name,
                    TableName = scoreboardEntity.TableName,
                    Email = scoreboardEntity.Email,
                    Token = Guid.Parse(scoreboardEntity.Token),
                    NumberOfEntries = scoreboardEntity.NumberOfEntries
                };

                return new OkObjectResult(scoreboard);
            }
            catch (Exception e)
            {
                log.LogError(e, "Failed to get scoreboard data");
                return new StatusCodeResult(500);
            }
        }
    }
}