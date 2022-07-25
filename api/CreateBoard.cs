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
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var createBoardRequest = JsonConvert.DeserializeObject<CreateBoardDto>(requestBody);

                var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
                var scoreboardTableClient = new TableClient(connectionString, "Scoreboard");

                var token = Guid.NewGuid().ToString();
                var tableName = RandomIdGenerator.GetBase62(8);
                try
                {
                    var newTableClient = new TableClient(connectionString, tableName);
                    await newTableClient.CreateAsync();
                    
                    // await scoreboardTableClient.AddEntityAsync(new BoardDataEntity
                    // {
                    //     PartitionKey = createBoardRequest.BoardName,
                    //     Email = createBoardRequest.Email,
                    //     Token = token,
                    //     NumberOfEntries = 100
                    // });
                    await scoreboardTableClient.AddEntityAsync(new BoardDataEntity
                    {
                        PartitionKey = token,
                        RowKey = "Data",
                        Name = createBoardRequest.BoardName,
                        TableName = tableName,
                        Email = createBoardRequest.Email,
                        Token = token,
                        NumberOfEntries = 100
                    });
                    await scoreboardTableClient.AddEntityAsync(new BoardNameEntity {
                        PartitionKey = tableName,
                        RowKey = "Name",
                        Token = token
                    });

                    // await scoreboardTableClient.AddEntityAsync(new BoardScoreCounterEntity
                    // {
                    //     PartitionKey = createBoardRequest.BoardName,
                    //     RowKey = "LowScore",
                    //     LowScore = -1
                    // });

                     await newTableClient.AddEntityAsync(new BoardScoreCounterEntity
                    {
                        PartitionKey = tableName,
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

                return new OkObjectResult(new ScoreboardDto {
                    Name = createBoardRequest.BoardName,
                    TableName = tableName,
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
