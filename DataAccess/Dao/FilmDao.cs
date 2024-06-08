using BussinessObject.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Dao
{
    public class FilmDao
    {
        private static FilmDao instance = null;
        private static object locked = new object();
        public static FilmDao GetFilmDao
        {
            get
            {
                lock (locked)
                {
                    if (instance == null)
                    {
                        instance = new FilmDao();
                    }
                    return instance;
                }

            }
        }

        public List<Film> getFilms()
        {
            var films = new List<Film>();
            try
            {
                using var dbcontext = new CinemaSystemContext();
                films = dbcontext.Films.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return films;
        }

        public List<Film> searchFilm(string q)
        {
            var films = new List<Film>();

            try
            {
                using var dbcontext = new CinemaSystemContext();
                films = dbcontext.Films.Where(e => e.Name.ToLower().Contains(q)).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

            return films;
        }


        public Film getFilmWithCategoriesShowsRoom(int filmId)
        {
            var films = new Film();
            try
            {
                using var context = new CinemaSystemContext();
                films = context.Films
               .Include(e => e.Categories)
               .Include(e => e.Shows)
               .ThenInclude(e => e.Room)
               .FirstOrDefault(e => e.Id == filmId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return films;
        }

        public List<Film> getFilm()
        {
            var films = new List<Film>();
            try
            {
                using var context = new CinemaSystemContext();
                films = context.Films.ToList();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return films;
        }

        public Film findById(int filmId)
        {
            var films = new Film();
            try
            {
                using var context = new CinemaSystemContext();
                films = context.Films.Find(filmId);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return films;
        }

        public void addFilm(Film film)
        {
            try
            {
                using var context = new CinemaSystemContext();
                context.Films.Add(film);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void removeFilm(Film film)
        {
            try
            {
                using var context = new CinemaSystemContext();
                context.Films.Remove(film);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
