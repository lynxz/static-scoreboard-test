namespace Scoreboard.Api.Request
{
    public class UploadScoreDto
    {

        public string UserName { get; set; }

        public long Score { get; set; }

        public string Token { get; set; }

    }
}
