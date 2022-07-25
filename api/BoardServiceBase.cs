using System;
using Azure.Data.Tables;
using Microsoft.Extensions.Logging;

namespace Scoreboard.Api
{
    public class BoardServiceBase
    {
        readonly string _connectionString;
        protected readonly string _boardName;
        protected readonly ILogger _logger;

        protected BoardServiceBase(string connectionString, ILogger logger, string boardName)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException($"'{nameof(connectionString)}' cannot be null or whitespace.", nameof(connectionString));
            }

            if (string.IsNullOrWhiteSpace(boardName))
            {
                throw new ArgumentException($"'{nameof(boardName)}' cannot be null or whitespace.", nameof(boardName));
            }

            _connectionString = connectionString;
            _boardName = boardName;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _logger.LogInformation($"{nameof(ScoreBoardService)} created.");
        }

        public TableClient Client { get; private set; }

        protected void Initialize()
        {
            Client = new TableClient(_connectionString, _boardName);
            _logger.LogInformation("Service initialized");
        }
    }




}