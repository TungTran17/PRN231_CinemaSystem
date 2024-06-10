using BussinessObject.Models;

namespace DataAccess.Repositories
{
    public interface IShowRepository
    {
        Show getShowWithFilmRoomTickets(int filmId);
        Show getShowWithRoomTickets(int filmId);
        Boolean checkvalidShow(Show show);
        void addShow(Show show);
    }
}
