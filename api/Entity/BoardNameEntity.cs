using System;
using Azure.Data.Tables;
using Azure;

namespace Scoreboard.Api
{
    public class BoardNameEntity : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
        public string Token { get; set; }
    }
}
