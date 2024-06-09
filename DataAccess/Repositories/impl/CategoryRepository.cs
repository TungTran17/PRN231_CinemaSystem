using BussinessObject.Models;
using DataAccess.Dao;

namespace DataAccess.Repositories.impl
{
    public class CategoryRepository : ICategoryRepository
    {
        public void addCategory(Category category) => CategoryDao.GetCategoryDao.addCategory(category);
        public Task<Category> getCategoryWithFilms(int id) => CategoryDao.GetCategoryDao.GetCategoryWithFilms(id);
        public List<Category> getListCategoriesbyFilm(List<int> categories) => CategoryDao.GetCategoryDao.getListCategoryOfFilm(categories);
        public List<Category> getListCategory() => CategoryDao.GetCategoryDao.getListCategory();
        public void removeCategory(Category category) => CategoryDao.GetCategoryDao.removeCategory(category);
        public void updateCategory(Category category) => CategoryDao.GetCategoryDao.updateCategory(category);
    }
}
