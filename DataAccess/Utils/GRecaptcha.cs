namespace CinemaSystemWebapp.Utils
{
    public static class GRecaptcha
    {
        public const string SITE_KEY = "6LfMaukiAAAAABd36Sv2HGjfv4Rs9IYWXT8aYoyq";
        public const string SECRET_KEY = "6LfMaukiAAAAACObPeOvRs7H3_z2FJ_1lVEXqzHO";

        public static async Task<bool> VerifyAsync(string gRecaptchaResponse)
        {
            using HttpClient client = new HttpClient();
            var result = await client.PostAsync(
                "https://www.google.com/recaptcha/api/siteverify",
                new FormUrlEncodedContent(new Dictionary<string, string> {
                    { "secret", SECRET_KEY},
                    { "response", gRecaptchaResponse},
                })
            );

            if (result.IsSuccessStatusCode)
            {
                var text = await result.Content.ReadAsStringAsync();
                return text.Contains("\"success\": true");
            }
            return false;
        }

        public static bool Verify(string gRecaptchaResponse)
        {
            return VerifyAsync(gRecaptchaResponse).Result;
        }
    }
}
