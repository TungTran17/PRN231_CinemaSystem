using BussinessObject.Models;
using DataAccess.Dao;

namespace DataAccess.Repositories.impl
{
    public class ShowRepository : IShowRepository
    {
        public void addShow(Show show) => ShowDao.GetShowDao.addShow(show);
        public Boolean checkvalidShow(Show show) => ShowDao.GetShowDao.checkValidShow(show);
        public List<Show> GetShows() => ShowDao.GetShowDao.GetAllShow();
        public Show getShowWithFilmRoomTickets(int filmId) => ShowDao.GetShowDao.getShowWithFilmRoomTickets(filmId);
        public Show getShowWithRoomTickets(int filmId) => ShowDao.GetShowDao.getShowWithRoomTickets(filmId);
        public bool removeShow(int showId) => ShowDao.GetShowDao.DeleteShow(showId);
    }
}
