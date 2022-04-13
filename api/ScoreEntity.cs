using System;
using Azure.Data.Tables;
using Azure;

namespace api
{
    public class ScoreEntity : ITableEntity
    {
        public long Score { get; set; }
        public string UserName { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
