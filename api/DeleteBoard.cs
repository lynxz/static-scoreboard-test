using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Web.Http;

namespace Scoreboard.Api
{
    public static class DeleteBoard
    {
        [FunctionName("DeleteBoard")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "deleteboard/{token}")] HttpRequest req,
            string token,
            ILogger log)
        {
            log.LogInformation($"Deleting board and scores for token {token}");
            try
            {
                var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
                var scoreBoardService = ScoreBoardService.Create(connectionString, log);
                var boardDataEntity = await scoreBoardService.GetBoardDataEntity(token);

                if (boardDataEntity.Token != token)
                    return new BadRequestObjectResult("Incorrect Token");

                if (!(await scoreBoardService.RemoveBoardEntities(token)))
                    return new InternalServerErrorResult();

                var scoreService = ScoreService.Create(connectionString, boardDataEntity.TableName, log);   
                await scoreService.DeleteTableAsync();

                return new OkResult();
            }
            catch (Exception e)
            {
                log.LogError(e, "Failed to delete scoreboard!");
                return new InternalServerErrorResult();
            }
        }
    }
}