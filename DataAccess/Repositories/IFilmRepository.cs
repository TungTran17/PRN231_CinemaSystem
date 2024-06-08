using BussinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
