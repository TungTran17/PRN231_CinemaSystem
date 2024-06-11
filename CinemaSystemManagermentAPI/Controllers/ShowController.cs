using DataAccess.Repositories.impl;
using DataAccess.Repositories;
using Microsoft.AspNetCore.Mvc;
using BussinessObject.Models;
using Microsoft.AspNetCore.OData.Query;

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
    }
}
