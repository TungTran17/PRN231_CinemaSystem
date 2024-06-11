using DataAccess.Repositories.impl;
using DataAccess.Repositories;
using Microsoft.AspNetCore.Mvc;
using BussinessObject.Models;
using Microsoft.AspNetCore.OData.Query;
using DataAccess.Utils;
using DataAccess.Dto;
using Microsoft.Extensions.Primitives;

namespace CinemaSystemManagermentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShowController : ControllerBase
    {
        IShowRepository _showRepository = new ShowRepository();
        ITicketRepository _ticketRepository = new TicketRepository();

        [EnableQuery]
        [HttpGet("{key}")]
        public ActionResult<Show> Get(int key)
        {
            var show = _showRepository.getShowWithFilmRoomTickets(key);
            if (show == null)
            {
                return NotFound("Show not found.");
            }
            return Ok(show);
        }

        [HttpPost("BuyTicket")]
        public IActionResult BuyTicket(int id, [FromBody] SeatDto seatDto)
        {
            User user;
            try
            {
                if (Request.Headers.TryGetValue("Authorization", out StringValues headerValue))
                {
                    var token = headerValue.FirstOrDefault()?.Split(' ').Last();
                    if (!string.IsNullOrEmpty(token))
                    {
                        user = Authentication.GetUserByToken(token);
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
            }
            catch (Exception ex)
            {
                return Unauthorized($"Authorization error: {ex.Message}");
            }

            if (user is null)
                return Unauthorized("User not authenticated.");

            var show = _showRepository.getShowWithRoomTickets(id);

            if (show is null || show.Room?.Rows < seatDto.Row || show.Room?.Cols < seatDto.Col)
                return BadRequest("Invalid show or wrong seat!");

            if (show.Tickets.Any(e => e.Row == seatDto.Row && e.Col == seatDto.Col))
                return Conflict("Sorry! Your seat has been ordered!");

            if (show.TicketPrice > user.Balance)
                return BadRequest("Your balance is not enough!");

            string GenOTP()
            {
                var rand = new Random();
                return new string(Enumerable.Range(0, 6).Select(_ => (char)rand.Next('0', '9' + 1)).ToArray());
            }

            DateTime currentTime = DateTime.Now;
            Ticket ticket = new Ticket()
            {
                UserId = user.Id,
                ShowId = id,
                Row = seatDto.Row,
                Col = seatDto.Col,
                Date = currentTime,
                Otp = GenOTP()
            };

            _ticketRepository.addNewTicket(ticket);

            return Ok("Buy Ticket Success!");
        }
    }
}
