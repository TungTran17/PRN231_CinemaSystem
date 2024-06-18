using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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

        [HttpPost]
        public async Task<ActionResult> Check(int showId, string email, string otp)
        {
            try
            {
                if (!HttpContext.Request.Cookies.TryGetValue("token", out string token))
                {
                    ViewBag.ErrorMessage = "User not signed in.";
                    ViewBag.Tickets = new List<CheckTicketViewDto>();
                    return await Index(0);
                }

                var request = new HttpRequestMessage(HttpMethod.Post, "https://localhost:7041/api/Staff/check-ticket");
                request.Headers.Add("Authorization", "Bearer " + token);
                var formData = new MultipartFormDataContent
                {
                    { new StringContent(showId.ToString()), "showId" },
                    { new StringContent(email), "email" },
                    { new StringContent(otp), "otp" }
                };
                request.Content = formData;

                var response = await _httpClient.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<CheckTicketResponse>(responseContent);

                await Index(0);

                if (response.IsSuccessStatusCode && result.Success)
                {
                    ViewBag.SuccessMessage = result.Message;
                    ViewBag.Tickets = new List<CheckTicketViewDto> { result.CheckTicket};
                }
                else
                {
                    ViewBag.ErrorMessage = result.Message;
                    ViewBag.Tickets = new List<CheckTicketViewDto>();
                }
                ViewBag.StaffUser = ViewBag.StaffUser ?? new User();
                ViewBag.Shows = ViewBag.Shows ?? new List<ShowDtoStaff>();
                return View("Index");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"An error occurred: {ex.Message}";
                ViewBag.Tickets = new List<CheckTicketViewDto>();
                await Index(0); 

                return View("Index");
            }
        }

    }
}
