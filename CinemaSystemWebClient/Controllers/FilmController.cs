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
                var uri = $"https://localhost:7041/odata/Film/{filmId}?$expand=Categories,Shows($expand=Room)";

                var response = await _httpClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var film = JsonConvert.DeserializeObject<Film>(json);

                if (film != null)
                {
                    ViewBag.Message = message;
                    ViewBag.Rooms = film.Shows.Select(s => s.Room).Distinct().ToList();
                    return View(film);
                }
                else
                {
                    ViewBag.ErrorMessage = "Film not found.";
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while processing your request.";
                Console.WriteLine("Exception: " + ex.Message);
                return View();
            }
        }
    }
}
