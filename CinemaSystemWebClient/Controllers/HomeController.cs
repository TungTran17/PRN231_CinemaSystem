﻿using BussinessObject.Models;
using DataAccess.OData;
using Microsoft.AspNetCore.Mvc;

namespace CinemaSystemWebClient.Controllers
{

    public class HomeController : Controller
    {
        private readonly HttpClient _httpClient;
        private const int PageSize = 12;
        public HomeController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var response = await _httpClient.GetFromJsonAsync<OdataResponsePage<Film>>($"https://localhost:7041/odata/Home?page={page}&pageSize={PageSize}");
            var films = response.Value;

            int totalCount = response.Count;
            int totalPages = (int)Math.Ceiling((double)totalCount / PageSize);

            ViewBag.Films = films;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View();
        }

        public async Task<IActionResult> Search(string q, int page = 1)
        {
            var response = await _httpClient.GetFromJsonAsync<OdataResponsePage<Film>>($"https://localhost:7041/api/Home/search?q={q}&page={page}&pageSize={PageSize}");
            var searchResults = response.Value;

            int totalCount = response.Count;
            int totalPages = (int)Math.Ceiling((double)totalCount / PageSize);

            ViewBag.Films = searchResults;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Signin()
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
            ViewBag.ErrorMessage = "Email or Password incorrect";
            return View();
        }

        public IActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Signup(string displayName, string email, string password, [FromForm(Name = "g-recaptcha-response")] string gRecaptchaResponse)
        {
            // Kiểm tra thời gian chờ: Đảm bảo rằng yêu cầu được gửi trong khoảng thời gian quy định
            if (Request.Form.Keys.Any(k => k.Equals("g-recaptcha-response")) && !Request.Form["g-recaptcha-response"].Equals(gRecaptchaResponse))
            {
                return BadRequest("Recaptcha verification failed: Timeout.");
            }

            // Xác định yêu cầu trùng lặp: Kiểm tra xem đã có yêu cầu xác thực trước đó không
            // Nếu có, từ chối yêu cầu mới và hiển thị thông báo lỗi
            if (HttpContext.Session.GetString("RecaptchaRequested") != null)
            {
                return BadRequest("Recaptcha verification failed: Duplicate request.");
            }

            // Lưu trạng thái của yêu cầu hiện tại vào session để xác định yêu cầu trùng lặp trong tương lai
            HttpContext.Session.SetString("RecaptchaRequested", "true");

            // Tạo form data để gửi lên server
            var formData = new Dictionary<string, string>
            {
                { "displayName", displayName },
                { "email", email },
                { "password", password },
                { "g-recaptcha-response", gRecaptchaResponse } // Chú ý tên của Recaptcha parameter
            };

            // Tạo nội dung yêu cầu từ form data
            var content = new FormUrlEncodedContent(formData);

            // Gửi yêu cầu đến API
            var response = await _httpClient.PostAsync("https://localhost:7041/api/Home/signup", content);

            // Xóa trạng thái yêu cầu sau khi yêu cầu được gửi
            HttpContext.Session.Remove("RecaptchaRequested");

            // Xử lý kết quả trả về từ API
            if (response.IsSuccessStatusCode)
            {
                // Đăng ký thành công, bạn có thể thực hiện các hành động khác ở đây (ví dụ: redirect đến trang chính)
                return View("Signin");
            }
            else
            {
                // Đăng ký không thành công, xử lý lỗi tại đây
                var errorMessage = await response.Content.ReadAsStringAsync();
                ViewBag.ErrorMessage = errorMessage;
                return View();
            }
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

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email, [FromForm(Name = "g-recaptcha-response")] string gRecaptchaResponse)
        {
            // Kiểm tra thời gian chờ và yêu cầu trùng lặp
            if (Request.Form.Keys.Any(k => k.Equals("g-recaptcha-response")) && !Request.Form["g-recaptcha-response"].Equals(gRecaptchaResponse))
            {
                ViewBag.ErrorMessage = "Recaptcha verification failed: Timeout.";
                return View();
            }

            if (HttpContext.Session.GetString("RecaptchaRequested") != null)
            {
                ViewBag.ErrorMessage = "Recaptcha verification failed: Duplicate request.";
                return View();
            }

            HttpContext.Session.SetString("RecaptchaRequested", "true");

            var formData = new Dictionary<string, string>
            {
                { "email", email },
                {"client-host", Request.Host.ToString()},
                { "g-recaptcha-response", gRecaptchaResponse }
            };

            var content = new FormUrlEncodedContent(formData);
            var response = await _httpClient.PostAsync("https://localhost:7041/api/Home/forgot-password", content);

            HttpContext.Session.Remove("RecaptchaRequested");

            if (response.IsSuccessStatusCode)
            {
                ViewBag.SuccessMessage = "A reset password link has been sent to your email.";
                return View("Signin");
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                ViewBag.ErrorMessage = errorMessage;
                return View();
            }
        }

        [HttpGet]
        public IActionResult ResetPassword(string token)
        {
            // Hiển thị trang Reset Password với token được truyền qua URL
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(string password, string confirmPassword, string token)
        {
            try
            {
                // Gửi yêu cầu reset password đến API
                var formData = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("password", password),
                    new KeyValuePair<string, string>("confirmPassword", confirmPassword),
                    new KeyValuePair<string, string>("token", token)
                });

                var response = await _httpClient.PostAsync("https://localhost:7041/api/Home/reset-password", formData);

                if (response.IsSuccessStatusCode)
                {
                    return View("Signin");
                }
                else
                {
                    ViewBag.ErrorMessage = "Failed to reset password. Please try again.";
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Failed to reset password: {ex.Message}";
                return View();
            }
        }

    }
}
