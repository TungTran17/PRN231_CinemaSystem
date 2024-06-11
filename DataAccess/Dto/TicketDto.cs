using BussinessObject.Models;

namespace DataAccess.Dto
{
    public class TicketDto
    {
        public Show Show { get; set; }
        public List<Ticket> Tickets { get; set; }
    }
}
