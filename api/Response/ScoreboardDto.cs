using System;

namespace Scoreboard.Api.Response
{
    public class ScoreboardDto
    {

        public string Name { get; set; }

        public string TableName { get; set; }

        public string Email { get; set; }

        public Guid Token { get; set; }

        public int NumberOfEntries { get; set; }

    }
}