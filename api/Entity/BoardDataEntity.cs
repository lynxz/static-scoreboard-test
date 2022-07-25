using System;
using Azure.Data.Tables;
using Azure;

namespace Scoreboard.Api
{
    public class BoardDataEntity : ITableEntity
    {
        public string PartitionKey { get; set; }

        public string RowKey { get; set; } = "Token";

        public DateTimeOffset? Timestamp { get; set; }

        public ETag ETag { get; set; }

        public string Name { get; set; }

        public string TableName { get; set; }

        public string Token { get; set; }

        public string Email { get; set; }

        public int NumberOfEntries { get; set; }
    }

}
