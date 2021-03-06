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

        public async Task<bool> CreateTableAsync() {
            try {
                await Client.CreateAsync();

                await Client.AddEntityAsync(new BoardScoreCounterEntity
                    {
                        PartitionKey = _boardName,
                        RowKey = "LowScore",
                        LowScore = -1
                    });
            } catch(Exception e) {
                _logger.LogError(e, $"Failed to create board {_boardName}");
                return false;
            }
            return true;
        }

        public async Task<bool> DeleteTableAsync() {
             try {
                await Client.DeleteAsync();
            } catch(Exception e) {
                _logger.LogError(e, $"Failed to delete board {_boardName}");
                return false;
            }
            return true;
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

         async Task<BoardScoreCounterEntity> GetScoreCounterEntityAsync(long score) {
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
                    _logger.LogError(ex, $"Failed to get lowScore for {_boardName}");
                    throw;
                }
            }
            return lowScore;
        }
    }

}