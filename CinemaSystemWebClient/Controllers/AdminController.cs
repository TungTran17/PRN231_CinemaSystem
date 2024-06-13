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

            var response = await _httpClient.PostAsync("https://localhost:7041/api/Admin/createShow", content);

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

        [HttpPost]
        public async Task<IActionResult> CRUDCategory(CategoryDto categoryDto)
        {
            var content = new StringContent(JsonConvert.SerializeObject(categoryDto), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("https://localhost:7041/api/Admin/category", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                return BadRequest(errorMessage);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CRUDFilm([FromForm] FilmDto filmDto, [FromForm] IFormFile? image)
        {
            using var formContent = new MultipartFormDataContent();
            string tab = "film";

            // Add the action to the form content
            formContent.Add(new StringContent(filmDto.Action), nameof(filmDto.Action));

            // Add necessary fields based on the action
            if (filmDto.Action == "create")
            {
                formContent.Add(new StringContent(filmDto.FilmName ?? string.Empty), nameof(filmDto.FilmName));
                formContent.Add(new StringContent(filmDto.Description ?? string.Empty), nameof(filmDto.Description));
                formContent.Add(new StringContent(filmDto.ReleaseDate.HasValue ? filmDto.ReleaseDate.Value.ToString("o") : string.Empty), nameof(filmDto.ReleaseDate));
                formContent.Add(new StringContent(filmDto.FilmLength.HasValue ? filmDto.FilmLength.Value.ToString() : string.Empty), nameof(filmDto.FilmLength));

                if (filmDto.Categories != null)
                {
                    foreach (var category in filmDto.Categories)
                    {
                        formContent.Add(new StringContent(category.ToString()), "categories"); // Sửa lại tên của categories
                    }
                }

                if (image != null)
                {
                    var streamContent = new StreamContent(image.OpenReadStream());
                    streamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                    {
                        Name = "image",
                        FileName = image.FileName
                    };
                    formContent.Add(streamContent, "image");
                }
            }
            else if (filmDto.Action == "delete")
            {
                formContent.Add(new StringContent(filmDto.Id.HasValue ? filmDto.Id.Value.ToString() : string.Empty), nameof(filmDto.Id));
            }

            var response = await _httpClient.PostAsync("https://localhost:7041/api/Admin/film", formContent);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index), new { tab = tab });
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                return BadRequest(errorMessage);
            }
        }




    }
}
