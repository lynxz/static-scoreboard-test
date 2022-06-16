using System;
using Azure.Data.Tables;
using Azure;

namespace Scoreboard.Api
{
    public class BoardDataEntity : ITableEntity
    {
        public long LowScore { get; set; }
        public int Count {get;set;}
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
