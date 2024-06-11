using BussinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;

namespace CinemaSystemWebClient.Controllers
{
    public class ShowController : Controller
    {
        private readonly HttpClient _httpClient;

        public ShowController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int id)
        {
            var uri = $"https://localhost:7041/odata/Show/{id}?$expand=Film,Room";
            var response = await _httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            if (!response.IsSuccessStatusCode)
            {
                ViewBag.ErrorMessage = "Error retrieving show details.";
                return View();
            }
            var json = await response.Content.ReadAsStringAsync();
            var show = JsonConvert.DeserializeObject<Show>(json);
            return View(show);
        }

    }
}
