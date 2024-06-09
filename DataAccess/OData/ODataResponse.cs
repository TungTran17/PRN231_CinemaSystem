using Newtonsoft.Json;

namespace DataAccess.OData
{
    public class ODataResponse<T>
    {
        [JsonProperty("@odata.context")]
        public string Context { get; set; }
        public T Value { get; set; }
    }
}
