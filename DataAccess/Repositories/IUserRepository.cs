using BussinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public interface IUserRepository
    {
        void AddToCard(User user, double money);
        void ChangePassword(User user, string newPassword, string confirmPassword, string oldPassword);
        User? FindByEmail(string email);
        User? Login(string email, string password);
        void ResetPassword(string password, string confirmPassword, User user);
        void Signup(User user);
        public void UpdateUser(User user, string name, string email, string? avatar);
    }
}
