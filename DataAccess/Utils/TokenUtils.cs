using Newtonsoft.Json;
using System.Security.Cryptography;

namespace CinemaSystemWebapp.Utils
{
    public static class TokenUtils
    {
        public static string CreateToken<T>(T data, string key)
        {
            string payload = JsonConvert.SerializeObject(data);
            string signature = Crypto.HashHMAC(payload, key);
            return $"{Crypto.Base64Encode(payload)}.{signature}";
        }

        public static T? DecryptToken<T>(string token, string key)
        {
            var parts = token.Split('.');

            if (parts.Length != 2) return default(T);

            string payload = Crypto.Base64Decode(parts[0]);
            string signature = parts[1];

            if (Crypto.HashHMAC(payload, key) != signature) return default(T);

            return JsonConvert.DeserializeObject<T>(payload);
        }

        public static T? DecryptTokenWithoutVerify<T>(string token)
        {
            var parts = token.Split('.');

            if (parts.Length != 2) return default(T);

            string payload = Crypto.Base64Decode(parts[0]);

            return JsonConvert.DeserializeObject<T>(payload);
        }
    }
}
