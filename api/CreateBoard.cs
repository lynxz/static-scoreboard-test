using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Azure.Data.Tables;
using System.Web.Http;
using System.IO;
using Newtonsoft.Json;
using Azure;
using Scoreboard.Api.Request;
using Scoreboard.Api.Response;

namespace Scoreboard.Api
{
    public static class CreateBoard
    {
        [FunctionName("CreateBoard")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "createboard")] HttpRequest req,
            ILogger log)
        {
            (bool success, CreateBoardDto createBoardRequest) = await req.GetDataAsync<CreateBoardDto>(log);
            if (!success)
                return new BadRequestErrorMessageResult("Failed to parse data");

            try
            {
                var token = Guid.NewGuid().ToString();
                var shortName = RandomIdGenerator.GetBase62(8);
                var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
                var scoreBoardService = ScoreBoardService.Create(connectionString, log);
                var scoreService = ScoreService.Create(connectionString, shortName, log);


                if (!(await scoreBoardService.CreateBoardEntities(token, createBoardRequest.BoardName, shortName, createBoardRequest.Email)))
                {
                    return new InternalServerErrorResult();
                }

                if (!(await scoreService.CreateTable()))
                {
                    await scoreBoardService.RemoveBoardEntities(token);
                    return new InternalServerErrorResult();
                }

                return new OkObjectResult(new ScoreboardDto
                {
                    Name = createBoardRequest.BoardName,
                    TableName = shortName,
                    Email = createBoardRequest.Email,
                    Token = Guid.Parse(token),
                    NumberOfEntries = 100,
                });
            }
            catch (Exception e)
            {
                log.LogError(e, "Failed to create scoreboard");
                return new InternalServerErrorResult();
            }
        }
    }
}
