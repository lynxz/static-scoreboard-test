using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Scoreboard.Api.Request;
using System.Web.Http;

namespace Scoreboard.Api
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

            (bool success, UpdateBoardDto updateBoard) = await req.GetDataAsync<UpdateBoardDto>(log);
            if (!success)
                return new BadRequestErrorMessageResult("Failed to parse data");

            try
            {
                var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
                var scoreBoardService = ScoreBoardService.Create(connectionString, log);

                var scoreboardEntity = await scoreBoardService.GetBoardDataEntity(token);
                if (scoreboardEntity.Token != token)
                    return new BadRequestObjectResult("Incorrect Token");

                if (updateBoard.NumberOfEntries < 1)
                    return new BadRequestObjectResult("Number of entries must be at least 1");

                scoreboardEntity.NumberOfEntries = updateBoard.NumberOfEntries;
                scoreboardEntity.Email = updateBoard.Email;

                var response = scoreBoardService.Client.UpdateEntity(scoreboardEntity, Azure.ETag.All);
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