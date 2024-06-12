using BussinessObject.Models;
using DataAccess.Repositories;
using DataAccess.Repositories.impl;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace CinemaSystemManagermentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilmController : ControllerBase
    {
        private readonly IFilmRepository _filmRepository = new FilmRepository();
        private readonly IRoomRepository _roomRepository = new RoomRepository();

        [EnableQuery]
        [HttpGet("{key}")]
        public async Task<ActionResult<Film>> Get(int key)
        {
            try
            {
                Film film = await _filmRepository.getFilmWithCategoriesShowsRoom(key);
                if (film == null)
                {
                    return NotFound();
                }
                return Ok(film);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while processing your request: {ex.Message}");
            }
        }

        [HttpGet("GetAllRoom")]
        public ActionResult<List<Room>> GetAllRoom()
        {
            try
            {
                List<Room> rooms = _roomRepository.getListRooms();

                if (rooms == null || rooms.Count == 0)
                {
                    return NotFound();
                }

                return Ok(rooms);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while processing your request: {ex.Message}");
            }
        }

    }
}
