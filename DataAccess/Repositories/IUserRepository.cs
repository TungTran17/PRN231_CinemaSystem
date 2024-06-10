using BussinessObject.Models;

namespace DataAccess.Repositories
{
    public interface IUserRepository
    {
        void AddToBaland(User user, double money);
        void ChangePassword(User user, string newPassword, string confirmPassword, string oldPassword);
        User? FindByEmail(string email);
        User? Login(string email, string password);
        void ResetPassword(string password, string confirmPassword, User user);
        void Signup(User user);
        public void UpdateUser(User user, string name, string email, string? avatar);
    }
}
