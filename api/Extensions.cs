using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Scoreboard.Api {

    public static class Extensions
    {
        
        public static async Task<(bool, T)> GetDataAsync<T>(this HttpRequest req, ILogger logger) {
            try {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data =  JsonConvert.DeserializeObject<T>(requestBody);
                return (true, data);
            } catch (Exception e) {
                logger.LogError(e, "Failed to parse request");
                return (false, default(T));
            }
        }

    }


}