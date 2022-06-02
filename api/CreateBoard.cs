using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Azure.Data.Tables;
using System.Linq;
using System.Web.Http;
using System.IO;
using Newtonsoft.Json;
using Azure;

namespace api
{
    public static class CreateBoard
    {
        [FunctionName("CreateBoard")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "createboard")] HttpRequest req,
            ILogger log)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var createBoardRequest = JsonConvert.DeserializeObject<CreateBoardRequestDto>(requestBody);

                var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
                var tableClient = new TableClient(connectionString, "Scoreboard");

                var token = Guid.NewGuid().ToString();
                try
                {
                    await tableClient.AddEntityAsync(new ScoreboardTokenEntity
                    {
                        PartitionKey = createBoardRequest.BoardName,
                        Email = createBoardRequest.Email,
                        Token = token,
                        NumberOfEntries = 100
                    });
                    await tableClient.AddEntityAsync(new BoardDataEntity
                    {
                        PartitionKey = createBoardRequest.BoardName,
                        RowKey = "LowScore",
                        LowScore = -1
                    });
                }
                catch (RequestFailedException requestFailed)
                {
                    if (requestFailed.Status == 409)
                        return new Microsoft.AspNetCore.Mvc.ConflictResult();

                    return new InternalServerErrorResult();
                }

                return new JsonResult(new { Name = createBoardRequest.BoardName, Token = token });
            }
            catch (Exception e)
            {
                log.LogError(e, "Failed to create scoreboard");
                return new InternalServerErrorResult();
            }
        }
    }
}
