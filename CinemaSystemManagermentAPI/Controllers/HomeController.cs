﻿using BussinessObject.Models;
using DataAccess.Repositories;
using DataAccess.Repositories.impl;
using DataAccess.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace CinemaSystemManagermentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IFilmRepository _filmRepository = new FilmRepository();
        private readonly IUserRepository _userRepository = new UserRepository();

        [EnableQuery]
        [HttpGet("films")]
        public ActionResult<List<Film>> Get()
        {
            return _filmRepository.GetFilms();
        }

        [EnableQuery]
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
                return RedirectToAction(nameof(Signup));
            }
            User user = new()
            {
                Email = email,
                Password = password,
                Name = email,
                Role = (int)BussinessObject.Models.User.Roles.User,
                AvatarUrl = "/assets/default.jpg"
            };
            try
            {
                _userRepository.Signup(user);
                return Ok();
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message); 
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); 
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest("Signup failed: " + ex.Message); // General error message
            }
        }


        [HttpPost("forgot-password")]
        public ActionResult ForgotPassword([FromForm] string email, [FromForm(Name = "g-recaptcha-response")] string gRecaptchaResponse, [FromForm(Name = "client-host")] string clientHost)
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
            var resetPasswordUrl = $"{Request.Scheme}://{clientHost}/Home/ResetPassword?token={token}";
            SMTP.Instance.Send("Reset Password", $"Here is your reset password link: {resetPasswordUrl}", email);
            return Ok();
        }

        [HttpGet("reset-password")]
        public ActionResult ResetPassword([FromQuery] string token)
        {
            var user = Authentication.GetUserByToken(token);
            if (user is null)
            {
                return RedirectToAction(nameof(Signout));
            }
            return Ok();
        }

        [HttpPost("reset-password")]
        public ActionResult ResetPassword([FromForm] string password, [FromForm] string confirmPassword, [FromForm] string token)
        {
            var user = Authentication.GetUserByToken(token);
            if (user is null)
            {
                return RedirectToAction(nameof(Signout));
            }
            try
            {
                _userRepository.ResetPassword(password, confirmPassword, user);
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(ResetPassword), new { token = token });
            }
            return Ok();
        }

        [HttpGet("signout")]
        public ActionResult Signout()
        {
            Response.Cookies.Delete("token");
            return Ok();
        }

    }
}
