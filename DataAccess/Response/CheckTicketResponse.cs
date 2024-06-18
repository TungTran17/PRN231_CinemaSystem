using DataAccess.Dto;
using Newtonsoft.Json;

namespace DataAccess.Response
{
    public class CheckTicketResponse
    {
        public CheckTicketViewDto CheckTicket { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
