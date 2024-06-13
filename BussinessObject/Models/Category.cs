using System.Text.Json.Serialization;

namespace BussinessObject.Models
{
    public partial class Category
    {
        public Category()
        {
            Films = new HashSet<Film>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Desc { get; set; } = null!;

        [JsonIgnore]
        public virtual ICollection<Film> Films { get; set; }
    }
}
