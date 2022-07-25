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

        public async Task<BoardNameEntity> GetBoardNameEntity(string boardName)
        {
            try
            {
                return (await Client.GetEntityAsync<BoardNameEntity>(boardName, "Name")).Value;
            }
            catch (Exception e)
            {
                _logger.LogError($"Could not get {nameof(BoardNameEntity)} for {boardName}", e);
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
                _logger.LogError($"Could not get {nameof(BoardDataEntity)} for {token}", e);
                throw;
            }
        }

    }


}