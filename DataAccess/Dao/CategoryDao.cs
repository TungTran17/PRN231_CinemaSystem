using BussinessObject.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Dao
{
    public class CategoryDao
    {
        private static CategoryDao instance = null;
        private static readonly object CategoryDaoLock = new object();

        public static CategoryDao GetCategoryDao
        {
            get
            {
                lock (CategoryDaoLock)
                {
                    if (instance == null)
                    {
                        instance = new CategoryDao();
                    }
                    return instance;
                }
            }
        }
        public List<Category> getListCategory()
        {
            var category = new List<Category>();
            try
            {
                using var context = new CinemaSystemContext();
                category = context.Categories.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return category;
        }

        public void addCategory(Category category)
        {
            try
            {
                using var context = new CinemaSystemContext();
                context.Categories.Add(category);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void updateCategory(Category category)
        {
            try
            {
                using var context = new CinemaSystemContext();
                context.Categories.Update(category);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void removeCategory(Category category)
        {
            try
            {
                using var context = new CinemaSystemContext();
                context.Categories.Remove(category);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public List<Category> getListCategoryOfFilm(List<int> categories)
        {
            try
            {
                using var context = new CinemaSystemContext();
                return context.Categories.Where(e => categories!.Contains(e.Id)).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<Category> GetCategoryWithFilms(int id)
        {
            using var dbcontext = new CinemaSystemContext();
            return await dbcontext.Categories.Include(e => e.Films).FirstOrDefaultAsync(e => e.Id == id);
        }

    }
}
