using BussinessObject.Models;
using DataAccess.Dto;
using DataAccess.Repositories;
using DataAccess.Repositories.impl;
using DataAccess.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace CinemaSystemManagermentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[ServiceFilter(typeof(AdminAuthorizationFilter))]
    public class AdminController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository = new CategoryRepository();
        private readonly IFilmRepository _filmRepository = new FilmRepository();
        private readonly IRoomRepository _roomRepository = new RoomRepository();
        private readonly IShowRepository _showRepository = new ShowRepository();
        private readonly ITicketRepository _ticketRepository = new TicketRepository();
        private readonly CinemaSystemContext dbcontext = new CinemaSystemContext();

        public User AdminUser { get; set; } = null!;

        [HttpGet("{tab?}")]
        public IActionResult Get(string? tab)
        {
            try
            {
                if (Request.Headers.TryGetValue("Authorization", out StringValues headerValue))
                {
                    var token = headerValue.FirstOrDefault()?.Split(' ').Last();
                    if (!string.IsNullOrEmpty(token))
                    {
                        AdminUser = Authentication.GetUserByToken(token);
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
                var categories = _categoryRepository.getListCategory();
                var films = _filmRepository.GetFilms();
                var orders = _ticketRepository.getAllTicket();
                var response = new { AdminUser, Categories = categories, Films = films, Orders = orders, ActiveTab = tab };
                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve,
                    WriteIndented = true
                };

                var jsonResponse = JsonSerializer.Serialize(response, options);

                var responseSize = jsonResponse.Length;
                Console.WriteLine($"Response size: {responseSize} bytes");

                return Ok(jsonResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("createShow")]
        public IActionResult CreateShow([FromBody] ShowDto showDto)
        {
            var film = _filmRepository.findById(showDto.Id);
            var roomObj = _roomRepository.getRoomById(showDto.Room);

            if (film is null || roomObj is null)
            {
                return BadRequest("Invalid film or room!");
            }

            var show = new Show
            {
                FilmId = film.Id,
                RoomId = roomObj.Id,
                Start = showDto.Start,
                End = showDto.Start.AddMinutes(film.Length),
                TicketPrice = showDto.Price
            };

            if (_showRepository.checkvalidShow(show))
            {
                return BadRequest("Show time is not valid!");
            }

            _showRepository.addShow(show);

            return Ok("Create show successful!");
        }

        [HttpPost("category")]
        public IActionResult Category([FromBody] CategoryDto categoryDto)
        {
            switch (categoryDto.Action)
            {
                case "create":
                    var categoryAdd = new Category { Name = categoryDto.Name, Desc = categoryDto.Description };
                    _categoryRepository.addCategory(categoryAdd);
                    break;
                case "edit":
                    if (categoryDto.Id.HasValue)
                    {
                        var categoryUpdate = new Category { Id = categoryDto.Id.Value, Name = categoryDto.Name, Desc = categoryDto.Description };
                        _categoryRepository.updateCategory(categoryUpdate);
                    }
                    break;
                case "delete":
                    if (categoryDto.Id.HasValue)
                    {
                        var categoryRemove = new Category { Id = categoryDto.Id.Value };
                        _categoryRepository.removeCategory(categoryRemove);
                    }
                    break;
                default:
                    return BadRequest("Invalid action!");
            }

            return Ok("Category action successful!");
        }

        [HttpPost("film")]
        public IActionResult Film([FromForm] FilmDto filmDto, [FromForm(Name = "image")] IFormFile? image)
        {
            switch (filmDto.Action)
            {
                case "create":
                    if (image != null)
                    {
                        var webClientRootPath = Path.Combine(Environment.CurrentDirectory, "..", "CinemaSystemWebClient", "wwwroot");
                        var uploadPath = Path.Combine(webClientRootPath, "assets");
                        using var stream = image.OpenReadStream();
                        var filepath = UploadFile.Upload(uploadPath, image.FileName, stream).FileName;

                        var newFilm = new Film
                        {
                            Name = filmDto.FilmName,
                            Desc = filmDto.Description,
                            Categories = dbcontext.Categories.Where(e => filmDto.Categories!.Contains(e.Id)).ToList(),
                            ReleaseDate = filmDto.ReleaseDate ?? DateTime.Now,
                            Length = filmDto.FilmLength ?? 0,
                            ImageUrl = $"/assets/{filepath}"
                        };

                        dbcontext.Films.Add(newFilm);
                        dbcontext.SaveChanges();

                        return Ok(newFilm);
                    }
                    else
                    {
                        return BadRequest("No image file provided.");
                    }
                case "delete":
                    if (filmDto.Id.HasValue)
                    {
                        var film = new Film { Id = filmDto.Id.Value };
                        _filmRepository.deleteFilm(film);
                    }
                    break;
                default:
                    return BadRequest("Invalid action!");
            }
            return Ok("Film action successful!");
        }
    }
}
