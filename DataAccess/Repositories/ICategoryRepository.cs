using BussinessObject.Models;

namespace DataAccess.Repositories
{
    public interface ICategoryRepository
    {
        List<Category> getListCategory();
        void addCategory(Category category);
        void updateCategory(Category category);
        List<Category> getListCategoriesbyFilm(List<int> categories);
        void removeCategory(Category category);
        Task<Category> getCategoryWithFilms(int id);
    }
}
