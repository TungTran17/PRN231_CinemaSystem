using DataAccess.Dto;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace CinemaSystemWebClient.Controllers
{
    public class AdminController : Controller
    {
        private readonly HttpClient _httpClient;

        public AdminController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet("Admin/Index")]
        public async Task<IActionResult> Index(string? tab)
        {
            try
            {
                if (!HttpContext.Request.Cookies.TryGetValue("token", out string token))
                {
                    ViewBag.ErrorMessage = "User not signed in.";
                    return View();
                }

                var request = new HttpRequestMessage(HttpMethod.Get, $"https://localhost:7041/api/Admin/{tab}");
                request.Headers.Add("Authorization", "Bearer " + token);

                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    ViewBag.ErrorMessage = $"Error retrieving data: {error}";
                    return View();
                }

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<AdminDto>(json);

                ViewBag.AdminUser = data.AdminUser;
                ViewBag.Categories = data.Categories;
                ViewBag.Films = data.Films;
                ViewBag.Orders = data.Orders;
                ViewBag.ActiveTab = data.ActiveTab;

                return View(data);
            }
            catch (HttpRequestException ex)
            {
                // Access the inner exception for more details
                var innerException = ex.InnerException;
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {innerException?.Message}");
            }

        }

        [HttpPost]
        public async Task<IActionResult> CreShow(int id, float price, DateTime start, int room)
        {
            var showData = new
            {
                Id = id,
                Price = price,
                Start = start,
                Room = room
            };

            var content = new StringContent(JsonConvert.SerializeObject(showData), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://localhost:7041/api/Admin/CreateShow", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                return BadRequest(errorMessage);
            }
        }
    }
}
