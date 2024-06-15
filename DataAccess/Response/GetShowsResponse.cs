using BussinessObject.Models;
using DataAccess.Dto;
using Newtonsoft.Json;

namespace DataAccess.Response
{
    public class GetShowsResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("shows")]
        public List<ShowDtoStaff> Shows { get; set; }

    }
}
