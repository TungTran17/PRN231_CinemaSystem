using BussinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CinemaSystemWebClient.Controllers
{
    public class CategoryController : Controller
    {
        private readonly HttpClient _httpClient;

        public CategoryController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [Route("Category/Index/{categoryId}")]
        public async Task<IActionResult> Index(int categoryId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"https://localhost:7041/odata/Category/{categoryId}?expand=Films");
                var json = await response.Content.ReadAsStringAsync();
                var category = JsonConvert.DeserializeObject<Category>(json);
                if (category != null && category.Films != null)
                {
                    ViewBag.CategoryName = category.Name;
                    ViewBag.CategoryDesc = category.Desc;
                    return View(category);
                }
                else
                {
                    ViewBag.ErrorMessage = "Category or films not found.";
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
