using BussinessObject.Models;

namespace DataAccess.Dto
{
    public class AdminDto
    {
        public User AdminUser { get; set; }
        public List<Category> Categories { get; set; }
        public List<Film> Films { get; set; }
        public List<Ticket> Orders { get; set; }
        public List<Show> Shows { get; set; }
        public string ActiveTab { get; set; }
    }
}
