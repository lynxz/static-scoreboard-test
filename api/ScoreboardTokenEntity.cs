using System;
using Azure.Data.Tables;
using Azure;

namespace api
{
    public class ScoreboardTokenEntity : ITableEntity
    {
        public string PartitionKey { get; set; }

        public string RowKey { get; set; } = "Token";

        public DateTimeOffset? Timestamp { get; set; }

        public ETag ETag { get; set; }

        public string Token { get; set; }

        public string Email { get; set; }

        public int NumberOfEntries { get; set; }
    }

}
