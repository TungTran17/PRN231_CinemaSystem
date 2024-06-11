using BussinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace CinemaSystemWebClient.Controllers
{
    public class UserController : Controller
    {
        private readonly HttpClient _httpClient;

        public UserController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [Route("User/Setting")]
        public async Task<IActionResult> Setting()
        {
            try
            {
                // Lấy cookie của tài khoản đã đăng nhập từ yêu cầu hiện tại
                var cookies = HttpContext.Request.Cookies;

                // Kiểm tra và lấy giá trị của cookie có tên là "token"
                if (!cookies.TryGetValue("token", out string token))
                {
                    ViewBag.ErrorMessage = "User not signed in.";
                    return View();
                }

                // Thực hiện yêu cầu HTTP với token được lấy từ cookie
                var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:7041/odata/User?$select=Email,Name,AvatarUrl,Balance,Tickets");
                request.Headers.Add("Authorization", "Bearer " + token);
                //Gửi đi
                var response = await _httpClient.SendAsync(request);

                // Xử lý phản hồi
                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.ErrorMessage = "User data not found.";
                    return View();
                }

                var json = await response.Content.ReadAsStringAsync();
                var user = JsonConvert.DeserializeObject<User>(json);

                return View(user);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"An error occurred: {ex.Message}";
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangeInfor(string name, string email, IFormFile? avatar)
        {
            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(name), "name");
            content.Add(new StringContent(email), "email");

            if (avatar != null)
            {
                var fileContent = new StreamContent(avatar.OpenReadStream());
                fileContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data")
                {
                    Name = "avatar",
                    FileName = avatar.FileName
                };
                content.Add(fileContent);
            }

            // Retrieve the token from cookies
            if (!HttpContext.Request.Cookies.TryGetValue("token", out string token))
            {
                ViewBag.ErrorMessage = "User not signed in.";
                return View("Setting");
            }

            // Create the request with the token in the header
            var request = new HttpRequestMessage(HttpMethod.Post, "https://localhost:7041/api/User/ChangeInfo")
            {
                Content = content
            };
            request.Headers.Add("Authorization", "Bearer " + token);

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Setting");
            }
            else
            {
                // Handle error
                var error = await response.Content.ReadAsStringAsync();
                ViewBag.ErrorMessage = $"Error: {error}";
                return View("Setting");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangePass(string oldPassword, string confirmPassword, string newPassword)
        {
            using var form = new MultipartFormDataContent();
            form.Add(new StringContent(oldPassword), "old-password");
            form.Add(new StringContent(confirmPassword), "confirm-password");
            form.Add(new StringContent(newPassword), "new-password");

            // Retrieve the token from cookies
            if (!HttpContext.Request.Cookies.TryGetValue("token", out string token))
            {
                ViewBag.ErrorMessage = "User not signed in.";
                return View("Setting");
            }

            // Create the request with the token in the header
            var request = new HttpRequestMessage(HttpMethod.Post, "https://localhost:7041/api/User/ChangePassword")
            {
                Content = form
            };
            request.Headers.Add("Authorization", "Bearer " + token);

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return View("Signin");
            }
            else
            {
                // Handle error
                var error = await response.Content.ReadAsStringAsync();
                ViewBag.ErrorMessage = error;
                return View("Setting");
            }
        }

       

    }
}
