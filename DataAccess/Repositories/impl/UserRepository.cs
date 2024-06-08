using BussinessObject.Models;
using DataAccess.Dao;

namespace DataAccess.Repositories.impl
{
    public class UserRepository : IUserRepository
    {
        public void AddToCard(User user, double money)
        {
            UserDao.Instance.AddToCard(user, money);
        }

        public void ChangePassword(User user, string newPassword, string confirmPassword, string oldPassword)
        {
            UserDao.Instance.changePassword(user, newPassword, confirmPassword, oldPassword);
        }

        public User? FindByEmail(string email)
        {
            return UserDao.Instance.findByEmail(email);
        }

        public User? Login(string email, string password)
        {
            return UserDao.Instance.Login(email, password);
        }

        public void ResetPassword(string password, string confirmPassword, User u)
        {
            UserDao.Instance.ResetPassword(password, confirmPassword, u);
        }

        public void Signup(User user)
        {
            UserDao.Instance.Signup(user);
        }

        public void UpdateUser(User user, string name, string email, string? avatar)
        {
            UserDao.Instance.updateUserInfor(user, name, email, avatar);
        }
    }
}
