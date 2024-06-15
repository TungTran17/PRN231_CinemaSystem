using Newtonsoft.Json;

namespace DataAccess.Response
{
    public class CheckTicketResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
