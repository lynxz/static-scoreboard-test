using System;

namespace api
{
    public class ScoreResponseDto
    {

        public string Board { get; set; }

        public string UserName { get; set; }

        public long Score { get; set; }

        public DateTime Date { get; set; }

    }
}