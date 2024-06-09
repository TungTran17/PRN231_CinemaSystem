namespace BussinessObject.Models
{
    public partial class Film
    {
        public Film()
        {
            Shows = new HashSet<Show>();
            Categories = new HashSet<Category>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Desc { get; set; } = null!;
        public int Length { get; set; }
        public string ImageUrl { get; set; } = null!;
        public DateTime ReleaseDate { get; set; }

        public virtual ICollection<Show> Shows { get; set; }

        public virtual ICollection<Category> Categories { get; set; }
    }
}
