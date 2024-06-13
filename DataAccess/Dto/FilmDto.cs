using System.Text.Json.Serialization;

namespace DataAccess.Dto
{
    public class FilmDto
    {
        public int? Id { get; set; }
        public string? FilmName { get; set; }
        public string? Description { get; set; }
        [JsonIgnore]
        public List<int>? Categories { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public int? FilmLength { get; set; }
        public string Action { get; set; }
    }
}
