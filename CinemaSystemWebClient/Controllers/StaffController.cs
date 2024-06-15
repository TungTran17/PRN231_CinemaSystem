using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static CinemaSystemManagermentAPI.Controllers.StaffController;
using System.Net.Http.Headers;
using DataAccess.Dto;
using DataAccess.Response;
using BussinessObject.Models;

namespace CinemaSystemWebClient.Controllers
{
    public class StaffController : Controller
    {
        private readonly HttpClient _httpClient;
        public StaffController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ActionResult> Index(int interval)
        {
            try
            {
                if (!HttpContext.Request.Cookies.TryGetValue("token", out string token))
                {
                    ViewBag.ErrorMessage = "User not signed in.";
                    return View();
                }

                var request = new HttpRequestMessage(HttpMethod.Get, $"https://localhost:7041/api/Staff/get-shows?interval={interval}");
                request.Headers.Add("Authorization", "Bearer " + token);

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<GetShowsResponse>(responseContent);

                if (result.Success)
                {
                    ViewBag.StaffUser = result.Shows.FirstOrDefault()?.StaffUser;
                    ViewBag.Shows = result.Shows;
                    return View();
                }

                ViewBag.ErrorMessage = result.Message;
                return View(new List<ShowDtoStaff>());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching shows: {ex.Message}");
                ViewBag.ErrorMessage = "An error occurred while fetching shows.";
                return View(new List<ShowDtoStaff>());
            }
        }

    }
}
