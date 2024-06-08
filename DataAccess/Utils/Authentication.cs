using BussinessObject.Models;
using DataAccess.Repositories;
using DataAccess.Repositories.impl;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace DataAccess.Utils
{
    public static class Authentication
    {
        private const string SECRET_KEY = "_r4nd0m_";
        private static readonly IUserRepository userRepository = new UserRepository();

        private struct AuthenticationToken
        {
            [JsonProperty("uid")]
            public int Id { get; set; }

            [JsonProperty("expiry")]
            public DateTime Expiry { get; set; }
        }

        public static User? GetUserByCookies(IRequestCookieCollection cookies)
        {
            var token = cookies.FirstOrDefault(e => e.Key == "token").Value;

            return GetUserByToken(token);
        }

        public static User? GetUserByToken(string token)
        {
            using CinemaSystemContext dbcontext = new();
            if (token is null)
                return null;

            var unsafe_data = TokenUtils.DecryptTokenWithoutVerify<AuthenticationToken>(token);

            var user = dbcontext.Users.FirstOrDefault(e => e.Id == unsafe_data.Id);

            if (user is null) return null;

            var data = TokenUtils.DecryptToken<AuthenticationToken>(token, SECRET_KEY + user.Password);

            if (data.Expiry < DateTime.Now)
                return null;


            return dbcontext.Users.FirstOrDefault(e => e.Id == data.Id);
        }

        public static string? CreateToken(string email, string password, TimeSpan expireIn)
        {
            User? user = userRepository.Login(email, password);
            if (user is null) return null;

            AuthenticationToken data = new AuthenticationToken
            {
                Id = user.Id,
                Expiry = DateTime.Now.Add(expireIn),
            };

            return TokenUtils.CreateToken(data, SECRET_KEY + user.Password);
        }

        public static string? CreateToken(string email, TimeSpan expireIn)
        {

            using CinemaSystemContext dbcontext = new CinemaSystemContext();
            User? user = userRepository.FindByEmail(email);


            if (user is null) return null;

            AuthenticationToken data = new AuthenticationToken
            {
                Id = user.Id,
                Expiry = DateTime.Now.Add(expireIn),
            };

            return TokenUtils.CreateToken(data, SECRET_KEY + user.Password);
        }
    }
}