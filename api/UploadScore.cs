using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Scoreboard.Api.Request;
using System.Web.Http;

namespace Scoreboard.Api
{
    public static class UploadScore
    {
        [FunctionName("UploadScore")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "uploadscore/{boardName}")] HttpRequest req,
            string boardName,
            ILogger log)
        {
            log.LogInformation($"Score uploaded from {req.HttpContext.Connection.RemoteIpAddress}.");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<UploadScoreDto>(requestBody);

                var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
                var scoreBoardService = ScoreBoardService.Create(connectionString, log);
                var scoreService = ScoreService.Create(connectionString, boardName, log);

                var scoreboardEntity = await scoreBoardService.GetBoardNameEntity(boardName);

                if (scoreboardEntity.Token != data.Token)
                {
                    return new BadRequestObjectResult("Invalid token");
                }

                await scoreService.AddScoreAsync(data);

                var scoreboardData = await scoreBoardService.GetBoardDataEntity(scoreboardEntity.Token);
                await scoreService.UpdateScoreCountersAsync(scoreboardData.NumberOfEntries, data.Score);
            }
            catch (Exception e)
            {
                log.LogError("Failed to upload score", e);
                return new InternalServerErrorResult();
            }

            return new OkResult();
        }
    }
}
