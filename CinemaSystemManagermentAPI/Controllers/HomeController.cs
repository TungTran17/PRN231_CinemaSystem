using BussinessObject.Models;
using DataAccess.Repositories;
using DataAccess.Repositories.impl;
using DataAccess.Utils;
using Microsoft.AspNetCore.Mvc;

namespace CinemaSystemManagermentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IFilmRepository _filmRepository = new FilmRepository();
        private readonly IUserRepository _userRepository = new UserRepository();

        [HttpGet("films")]
        public ActionResult<List<Film>> GetFilms()
        {
            return _filmRepository.GetFilms();
        }

        [HttpGet("search")]
        public ActionResult<List<Film>> Search([FromQuery] string q)
        {
            q = q?.ToLower() ?? "";
            return _filmRepository.SearchFilm(q);
        }

        [HttpPost("signin")]
        public IActionResult Signin([FromForm] string email, [FromForm] string password)
        {
            string? token = Authentication.CreateToken(email, password, TimeSpan.FromDays(1));
            if (token is null)
            {
                return RedirectToAction(nameof(Signin));
            }
            Response.Cookies.Append("token", token);
            return Ok(token);
        }

        [HttpPost("signup")]
        public ActionResult Signup([FromForm] string email, [FromForm] string password, [FromForm(Name = "g-recaptcha-response")] string gRecaptchaResponse)
        {
            if (!GRecaptcha.Verify(gRecaptchaResponse))
            {
                return BadRequest("Recaptcha verification failed.");
            }
            User user = new()
            {
                Email = email,
                Password = password,
                Name = email,
                Role = ((int)BussinessObject.Models.User.Roles.User),
                AvatarUrl = "/assets/default.jpg"
            };
            try
            {
                _userRepository.Signup(user);
            }
            catch (Exception ex)
            {
                return BadRequest("Signup failed.");
            }
            return Ok();
        }

        [HttpPost("forgot-password")]
        public ActionResult ForgotPassword([FromForm] string email, [FromForm(Name = "g-recaptcha-response")] string gRecaptchaResponse)
        {
            if (!GRecaptcha.Verify(gRecaptchaResponse))
            {
                return BadRequest("Recaptcha verification failed.");
            }

            if (_userRepository.FindByEmail(email) is null)
            {
                return BadRequest("Email not found.");
            }
            string? token = Authentication.CreateToken(email, TimeSpan.FromMinutes(30));
            if (token is null)
            {
                return BadRequest("Token creation failed.");
            }
            string generatedLink = $"{Request.Host}/Home/ResetPassword?token={token}";
            SMTP.Instance.Send("Reset Password", $"Here is your reset password link: {generatedLink}", email);
            return Ok();
        }

        [HttpGet("reset-password")]
        public ActionResult ResetPassword([FromQuery] string token)
        {
            var user = Authentication.GetUserByToken(token);
            if (user is null)
            {
                return Unauthorized();
            }
            return Ok(); // Return something indicating the user can reset the password.
        }

        [HttpPost("reset-password")]
        public ActionResult ResetPassword([FromForm] string password, [FromForm] string confirmPassword, [FromForm] string token)
        {
            var user = Authentication.GetUserByToken(token);
            if (user is null)
            {
                return Unauthorized();
            }
            try
            {
                _userRepository.ResetPassword(password, confirmPassword, user);
            }
            catch (Exception ex)
            {
                return BadRequest("Password reset failed.");
            }
            return Ok();
        }

        [HttpGet("signout")]
        public ActionResult Signout()
        {
            Response.Cookies.Delete("token");
            return Ok();
        }

        [HttpGet("privacy")]
        public ActionResult<string> Privacy()
        {
            return "Privacy Policy";
        }
    }
}
