using BussinessObject.Models;
using DataAccess.Dao;

namespace DataAccess.Repositories.impl
{
    public class TicketRepository : ITicketRepository
    {
        public List<Ticket> GetListTicketWithFullInformation() => TicketDao.GetTicketDao.GetListTicketWitFullInfomation();
        public void addNewTicket(Ticket ticket) => TicketDao.GetTicketDao.addNewTicket(ticket);
        public List<Ticket> getAllTicket() => TicketDao.GetTicketDao.getAllTicket();
    }
}
