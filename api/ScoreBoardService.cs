using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Scoreboard.Api
{

    public class ScoreBoardService : BoardServiceBase
    {
        const string BOARD_NAME = "Scoreboard";

        private ScoreBoardService(string connectionString, ILogger logger, string boardName) : base(connectionString, logger, boardName)
        {
        }

        public static ScoreBoardService Create(string connectionString, ILogger logger)
        {
            var service = new ScoreBoardService(connectionString, logger, BOARD_NAME);
            service.Initialize();
            return service;
        }

        public async Task<bool> CreateBoardEntities(string token, string name, string shortName, string email)
        {
            try
            {
                await Client.AddEntityAsync(new BoardDataEntity
                {
                    PartitionKey = token,
                    RowKey = "Data",
                    Name = name,
                    TableName = shortName,
                    Email = email,
                    Token = token,
                    NumberOfEntries = 100
                });
                await Client.AddEntityAsync(new BoardNameEntity
                {
                    PartitionKey = shortName,
                    RowKey = "Name",
                    Token = token
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to create board entities for {name}");
                return false;
            }


            return true;
        }

        public async Task<bool> RemoveBoardEntities(string token)
        {
            try
            {
                var boardDataEntity = (await Client.GetEntityAsync<BoardDataEntity>(token, "Data"))?.Value;
                await Client.DeleteEntityAsync(token, "Data");
                await Client.DeleteEntityAsync(boardDataEntity.TableName, "Name");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to delete board entities for token {token}");
                return false;
            }

            return true;
        }

        public async Task<BoardNameEntity> GetBoardNameEntity(string boardName)
        {
            try
            {
                return (await Client.GetEntityAsync<BoardNameEntity>(boardName, "Name")).Value;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Could not get {nameof(BoardNameEntity)} for {boardName}");
                throw;
            }
        }

        public async Task<BoardDataEntity> GetBoardDataEntity(string token)
        {
            try
            {
                return (await Client.GetEntityAsync<BoardDataEntity>(token, "Data")).Value;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Could not get {nameof(BoardDataEntity)} for {token}");
                throw;
            }
        }

    }


}