using BussinessObject.Models;
using DataAccess.Dao;

namespace DataAccess.Repositories
{
    public interface ITicketRepository
    {
        List<Ticket> GetListTicketWithFullInformation();
        public List<Ticket> getAllTicket();
        public void addNewTicket(Ticket ticket) => TicketDao.GetTicketDao.addNewTicket(ticket);
    }
}
