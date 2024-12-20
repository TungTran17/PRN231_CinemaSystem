﻿using BussinessObject.Models;
using DataAccess.Repositories.impl;
using DataAccess.Repositories;
using DataAccess.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.Extensions.Primitives;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace CinemaSystemManagermentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository = new UserRepository();
        private readonly ITicketRepository _ticketRepository = new TicketRepository();

        [EnableQuery]
        [HttpGet("Setting")]
        public IActionResult Get()
        {
            // Check Authorization có được gửi không
            if (Request.Headers.TryGetValue("Authorization", out StringValues headerValue))
            {
                // gán token
                var token = headerValue.FirstOrDefault()?.Split(' ').Last();
                if (!string.IsNullOrEmpty(token))
                {
                    var user = Authentication.GetUserByToken(token);
                    if (user != null)
                    {
                        return Ok(user);
                    }
                }
                return Unauthorized("Invalid token.");
            }
            var userFromCookies = Authentication.GetUserByCookies(Request.Cookies);
            if (userFromCookies == null)
            {
                return Unauthorized("User not signed in.");
            }
            return Ok(userFromCookies);
        }


        [HttpGet("Tickets")]
        public IActionResult GetTickets()
        {
            try
            {
                if (Request.Headers.TryGetValue("Authorization", out StringValues headerValue))
                {
                    var token = headerValue.FirstOrDefault()?.Split(' ').Last();
                    if (!string.IsNullOrEmpty(token))
                    {
                        var user = Authentication.GetUserByToken(token);
                        if (user != null)
                        {
                            List<Ticket> list = _ticketRepository.GetListTicketWithFullInformation();
                            var userTickets = list.Where(e => e.UserId == user.Id).GroupBy(e => e.Show)
                                                  .Select(g => new
                                                  {
                                                      Show = g.Key,
                                                      Tickets = g.ToList()
                                                  }).ToList();

                            var options = new JsonSerializerOptions
                            {
                                ReferenceHandler = ReferenceHandler.Preserve,
                                WriteIndented = true
                            };

                            return new JsonResult(userTickets, options);
                        }
                        else
                        {
                            return Unauthorized("Invalid token.");
                        }
                    }
                }

                // Nếu không có tiêu đề "Authorization" hoặc không có token hợp lệ, trả về thông báo thành công
                return Unauthorized("Authorization header or token is missing.");
            }
            catch (Exception ex)
            {
                // Ghi lại lỗi
                // (Bạn có thể sử dụng logging framework như Serilog, NLog, etc.)
                Console.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode(500, "An internal server error occurred.");
            }
        }


        [HttpPost("ChangeInfo")]
        public IActionResult ChangeInfo([FromForm] string name, [FromForm] string email, [FromForm] IFormFile? avatar)
        {
            // Check Authorization header
            if (Request.Headers.TryGetValue("Authorization", out StringValues headerValue))
            {
                var token = headerValue.FirstOrDefault()?.Split(' ').Last();

                if (!string.IsNullOrEmpty(token))
                {
                    var user = Authentication.GetUserByToken(token);
                    if (user != null)
                    {
                        string? newAvatar = null;
                        if (avatar != null)
                        {
                            var webClientRootPath = Path.Combine(Environment.CurrentDirectory, "..", "CinemaSystemWebClient", "wwwroot");
                            var uploadPath = Path.Combine(webClientRootPath, "assets");
                            using var stream = avatar.OpenReadStream();
                            newAvatar = UploadFile.Upload(uploadPath, avatar.FileName, stream).FileName;
                        }
                        _userRepository.UpdateUser(user, name, email, newAvatar);
                        return Ok("User information updated successfully.");
                    }

                    return Unauthorized("Invalid token.");
                }
            }

            var userFromCookies = Authentication.GetUserByCookies(Request.Cookies);
            if (userFromCookies == null)
            {
                return Unauthorized("User not signed in.");
            }

            return Ok("User information updated successfully.");
        }

        [HttpPost("ChangePassword")]
        public IActionResult ChangePassword([FromForm(Name = "old-password")] string oldPassword, [FromForm(Name = "confirm-password")] string confirmPassword, [FromForm(Name = "new-password")] string newPassword)
        {
            if (Request.Headers.TryGetValue("Authorization", out StringValues headerValue))
            {
                var token = headerValue.FirstOrDefault()?.Split(' ').Last();

                if (!string.IsNullOrEmpty(token))
                {
                    var user = Authentication.GetUserByToken(token);
                    _userRepository.ChangePassword(user, newPassword, confirmPassword, oldPassword);
                }
                else
                {
                    return Unauthorized("Invalid token.");
                }
            }
            return Ok("Password changed successfully.");
        }
    }
}
