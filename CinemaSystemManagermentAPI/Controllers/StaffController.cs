using BussinessObject.Models;
using DataAccess.Dto;
using DataAccess.Response;
using DataAccess.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;

namespace CinemaSystemManagermentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffController : ControllerBase
    {
        private readonly CinemaSystemContext _context = new CinemaSystemContext();
        public User StaffUser { get; set; } = null!;

        [HttpGet("get-shows")]
        public ActionResult<GetShowsResponse> GetShows([FromQuery] int interval)
        {
            try
            {
                if (Request.Headers.TryGetValue("Authorization", out StringValues headerValue))
                {
                    var token = headerValue.FirstOrDefault()?.Split(' ').Last();
                    if (!string.IsNullOrEmpty(token))
                    {
                        StaffUser = Authentication.GetUserByToken(token);
                    }
                    else
                    {
                        return Unauthorized("Invalid token.");
                    }
                }
                else
                {
                    return Unauthorized("Authorization header not found.");
                }

                var intervalTS = TimeSpan.FromMilliseconds(interval);
                var startTime = DateTime.Now;
                var endTime = DateTime.Now.Add(intervalTS);

                return new GetShowsResponse()
                {
                    Success = true,
                    Message = "Shows successfully retrieved",
                    Shows = _context.Shows
                        .Include(s => s.Film)
                        .Include(s => s.Room)
                        .Select(s => new ShowDtoStaff()
                        {
                            Id = s.Id,
                            Film = s.Film!.Name,
                            Start = s.Start,
                            End = s.End,
                            Price = (float)s.TicketPrice,
                            Room = s.Room!.Name,
                            StaffUser = StaffUser
                        })
                        .Where(s => (s.End > startTime && s.End < endTime) || (s.Start > startTime && s.Start < endTime) || (s.Start < startTime && s.End > endTime))
                        .ToList()
                };
            }
            catch (Exception ex)
            {
                return new GetShowsResponse()
                {
                    Success = false,
                    Message = ex.Message,
                    Shows = new List<ShowDtoStaff>()
                };
            }
        }

        [HttpPost("check-ticket")]
        public ActionResult<CheckTicketResponse> CheckTicket([FromForm] int showId, [FromForm] string email, [FromForm] string otp)
        {
            try
            {
                if (Request.Headers.TryGetValue("Authorization", out StringValues headerValue))
                {
                    var token = headerValue.FirstOrDefault()?.Split(' ').Last();
                    if (!string.IsNullOrEmpty(token))
                    {
                        StaffUser = Authentication.GetUserByToken(token);
                    }
                    else
                    {
                        return Unauthorized("Invalid token.");
                    }
                }
                else
                {
                    return Unauthorized("Authorization header not found.");
                }

                if (StaffUser is null)
                {
                    return new CheckTicketResponse()
                    {
                        Success = false,
                        Message = "Invalid token"
                    };
                }
                if (StaffUser.Role == (int)BussinessObject.Models.User.Roles.User)
                {
                    return new CheckTicketResponse()
                    {
                        Success = false,
                        Message = "You don't have permission to do this"
                    };
                }
                var ticket = _context.Tickets
                            .Include(e => e.User)
                            .Include(e => e.Show)
                                .ThenInclude(s => s.Film)
                            .Include(e => e.Show)
                                .ThenInclude(s => s.Room)
                            .FirstOrDefault(e => e.User!.Email == email && e.ShowId == showId && e.Otp == otp && !e.IsUsed);

                if (ticket is null)
                {
                    return new CheckTicketResponse()
                    {
                        Success = false,
                        Message = "Invalid ticket"
                    };
                }

                ticket.IsUsed = true;
                _context.SaveChanges();

                var checkTicketDto = new CheckTicketViewDto
                {
                    UserName = ticket.User.Name,
                    Film = ticket.Show.Film.Name,
                    Room = ticket.Show.Room.Name,
                    Row = ticket.Row,
                    Col = ticket.Col,
                    Date = ticket.Show.Start,
                    Status = ticket.IsUsed
                };

                return new CheckTicketResponse()
                {
                    CheckTicket = checkTicketDto,
                    Success = true,
                    Message = "Ticket successfully checked"
                };
            }
            catch (Exception ex)
            {
                return new CheckTicketResponse()
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
    }
}
