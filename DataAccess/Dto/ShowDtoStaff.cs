using BussinessObject.Models;
using Newtonsoft.Json;

namespace DataAccess.Dto
{
    public class ShowDtoStaff
    {
        public int Id { get; set; }
        public float Price { get; set; }
        public DateTime Start { get; set; }
        public string Room { get; set; }
        public string Film { get; set; }
        public DateTime End { get; set; }
        public User StaffUser { get; set; }
    }
}
