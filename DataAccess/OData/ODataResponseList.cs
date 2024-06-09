using Newtonsoft.Json;

namespace DataAccess.OData
{
    public class ODataResponseList<T>
    {
        [JsonProperty("@odata.context")]
        public string Context { get; set; }
        public List<T> Value { get; set; }
    }
}
