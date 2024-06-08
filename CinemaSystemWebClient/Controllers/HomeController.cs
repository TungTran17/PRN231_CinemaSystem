using BussinessObject.Models;
using Microsoft.AspNetCore.Mvc;

namespace CinemaSystemWebClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _httpClient;

        public HomeController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IActionResult> Index()
        {
            var films = await _httpClient.GetFromJsonAsync<List<Film>>("https://localhost:7041/api/Home/films");
            ViewBag.Films = films;
            return View(films);
        }
    }
}
