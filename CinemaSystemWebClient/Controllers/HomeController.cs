using BussinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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

        public async Task<IActionResult> Search(string q)
        {
            var searchResults = await _httpClient.GetFromJsonAsync<List<Film>>($"https://localhost:7041/api/Home/search?q={q}");
            ViewBag.Films = searchResults;
            return View(searchResults);
        }

        public IActionResult Signin()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Signin(string email, string password)
        {

            // Tạo form data để gửi lên server
            var formData = new Dictionary<string, string>
                {
                    { "email", email },
                    { "password", password }
                };

            // Tạo nội dung yêu cầu từ form data
            var content = new FormUrlEncodedContent(formData);

            var response = await _httpClient.PostAsync("https://localhost:7041/api/Home/signin", content);

            if (response.IsSuccessStatusCode)
            {
                var token = await response.Content.ReadAsStringAsync();
                // Lưu token vào Session hoặc Cookie
                // Sử dụng Cookie
                Response.Cookies.Append("token", token);
                // Sử dụng Session
                //HttpContext.Session.SetString("token", token);
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        public IActionResult Signup()
        {
            return View();
        }

        public async Task<IActionResult> Signout()
        {
            var response = await _httpClient.GetAsync("https://localhost:7041/api/Home/signout");

            if (response.IsSuccessStatusCode)
            {
                // Xóa token từ cookie
                Response.Cookies.Delete("token");
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

    }
}
