using BussinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CinemaSystemWebClient.Controllers
{
    public class FilmController : Controller
    {
        private readonly HttpClient _httpClient;

        public FilmController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [Route("Film/Index/{filmId}")]
        public async Task<IActionResult> Index(int filmId, string? message)
        {
            try
            {
                var filmUri = $"https://localhost:7041/odata/Film/{filmId}?$expand=Categories,Shows($expand=Room)";
                var filmResponse = await _httpClient.GetAsync(filmUri);
                filmResponse.EnsureSuccessStatusCode();

                var filmJson = await filmResponse.Content.ReadAsStringAsync();
                var film = JsonConvert.DeserializeObject<Film>(filmJson);

                var roomsUri = "https://localhost:7041/api/Film/GetAllRoom";
                var roomsResponse = await _httpClient.GetAsync(roomsUri);
                roomsResponse.EnsureSuccessStatusCode();

                var roomsJson = await roomsResponse.Content.ReadAsStringAsync();
                var allRooms = JsonConvert.DeserializeObject<List<Room>>(roomsJson);

                if (film != null)
                {
                    ViewBag.Message = message;
                    ViewBag.Rooms = allRooms;
                    return View(film);
                }
                else
                {
                    ViewBag.ErrorMessage = "Film not found.";
                    ViewBag.Rooms = allRooms;
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while processing your request.";
                ViewBag.Rooms = new List<Room>();
                Console.WriteLine("Exception: " + ex.Message);
                return View();
            }
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

    }
}
