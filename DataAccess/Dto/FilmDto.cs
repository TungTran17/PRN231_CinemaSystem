namespace DataAccess.Dto
{
    public class FilmDto
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<int>? Categories { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public int? Length { get; set; }
        public string Action { get; set; }
    }
}
