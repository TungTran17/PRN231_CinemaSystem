using BussinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

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

        [HttpPost]
        public async Task<IActionResult> BuyTickets(int id, int row, int col)
        {
            var seatDto = new { Row = row, Col = col };
            var content = new StringContent(JsonConvert.SerializeObject(seatDto), Encoding.UTF8, "application/json");

            var token = Request.Cookies["token"];
            if (string.IsNullOrEmpty(token))
            {
                ViewBag.ErrorMessage = "User not signed in.";
                return View("Signin");
            }

            var request = new HttpRequestMessage(HttpMethod.Post, $"https://localhost:7041/api/Show/BuyTicket?id={id}");
            request.Content = content;
            request.Headers.Add("Authorization", "Bearer " + token);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.ErrorMessage = "Error buying ticket.";
                return View();
            }

            return RedirectToAction("Index", new { id = id, message = "Buy Ticket Success!" });
        }
    }
}
