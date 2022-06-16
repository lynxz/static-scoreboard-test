using System;

namespace Scoreboard.Api.Response
{
    public class ScoreDto
    {

        public string Board { get; set; }

        public string UserName { get; set; }

        public long Score { get; set; }

        public DateTime Date { get; set; }

    }
}
