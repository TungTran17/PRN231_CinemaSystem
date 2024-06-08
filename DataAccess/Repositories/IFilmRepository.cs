using BussinessObject.Models;

namespace DataAccess.Repositories
{
    public interface IFilmRepository
    {
        Film getFilmWithCategoriesShowsRoom(int filmId);
        List<Film> GetFilms();
        Film findById(int filmId);
        void createFilm(Film film);
        void deleteFilm(Film film);
        List<Film> SearchFilm(string q);
    }
}
