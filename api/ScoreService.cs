using System;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Microsoft.Extensions.Logging;
using Scoreboard.Api.Request;

namespace Scoreboard.Api
{
    public class ScoreService : BoardServiceBase
    {
        private ScoreService(string connectionString, ILogger logger, string boardName) : base(connectionString, logger, boardName)
        {
        }

        public static ScoreService Create(string connectionString, string boardName, ILogger logger)
        {
            var service = new ScoreService(connectionString, logger, boardName);
            service.Initialize();
            return service;
        }

        public async Task AddScoreAsync(UploadScoreDto data) {
            var scoreEntity = new ScoreEntity
            {
                Score = data.Score,
                UserName = data.UserName,
                PartitionKey = _boardName,
                RowKey = Guid.NewGuid().ToString()
            };

            await Client.AddEntityAsync(scoreEntity);
        }

        

        public async Task UpdateScoreCountersAsync(int numberOfEntries, long score) {
            var scoreCounter = await GetScoreCounterEntityAsync(score);

            if (scoreCounter.LowScore < score)
            {
                var entries = await Client.QueryAsync<ScoreEntity>(c => c.PartitionKey == _boardName && c.RowKey != "LowScore" && c.Score >= scoreCounter.LowScore).ToListAsync();
                var lowest = entries.OrderByDescending(s => s.Score).Take(numberOfEntries).Last();
                scoreCounter.LowScore = lowest.Score;

                if (scoreCounter.Count < numberOfEntries)
                    scoreCounter.Count++;

                await Client.UpdateEntityAsync(scoreCounter, ETag.All);
            }
            else if (scoreCounter.Count < numberOfEntries)
            {
                var entries = await Client.QueryAsync<ScoreEntity>(c => c.PartitionKey == _boardName && c.RowKey != "LowScore").ToListAsync();
                var lowest = entries.OrderByDescending(s => s.Score).Take(numberOfEntries).Last();
                scoreCounter.LowScore = lowest.Score;
                scoreCounter.Count++;

                await Client.UpdateEntityAsync(scoreCounter, ETag.All);
            }
        }

        public async Task<BoardScoreCounterEntity> GetScoreCounterEntityAsync(long score) {
            BoardScoreCounterEntity lowScore = null;
            try
            {
                lowScore = (await Client.GetEntityAsync<BoardScoreCounterEntity>(_boardName, "LowScore")).Value;
                _logger.LogInformation($"LowScore entry found for board {_boardName}");
            }
            catch (RequestFailedException ex)
            {
                if (ex.Status == 404)
                {
                    lowScore = new BoardScoreCounterEntity { PartitionKey = _boardName, RowKey = "LowScore", LowScore = score };
                    await Client.AddEntityAsync(lowScore);
                }
                else
                {
                    _logger.LogError($"Failed to get lowScore for {_boardName}", ex);
                    throw;
                }
            }
            return lowScore;
        }
    }
    
}