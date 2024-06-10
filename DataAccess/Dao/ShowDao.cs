using BussinessObject.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Dao
{
    public class ShowDao
    {
        private static ShowDao instance = null;
        private static readonly object showDaoLock = new object();

        public static ShowDao GetShowDao
        {
            get
            {
                lock (showDaoLock)
                {
                    if (instance == null)
                    {
                        instance = new ShowDao();
                    }
                    return instance;
                }
            }
        }

        public Show getShowWithFilmRoomTickets(int filmId)
        {
            var show = new Show();
            try
            {
                using var context = new CinemaSystemContext();
                show = context.Shows.Include(e => e.Film)
                .Include(e => e.Room)
                .Include(e => e.Tickets)
                .FirstOrDefault(e => e.Id == filmId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return show;
        }


        public Show getShowWithRoomTickets(int filmId)
        {
            var show = new Show();
            try
            {
                using var context = new CinemaSystemContext();
                show = context.Shows.Include(e => e.Room)
                .Include(e => e.Tickets)
                .FirstOrDefault(e => e.Id == filmId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return show;
        }


        public Boolean checkValidShow(Show show)
        {
            try
            {
                using var context = new CinemaSystemContext();
                return context.Shows.Any(s => s.RoomId == show.RoomId && ((s.End > show.Start && s.End < show.End) ||
              (s.Start > show.Start && s.Start < show.End) || (s.Start < show.Start && s.End > show.End)));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void addShow(Show show)
        {
            try
            {
                using var context = new CinemaSystemContext();
                context.Shows.Add(show);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
