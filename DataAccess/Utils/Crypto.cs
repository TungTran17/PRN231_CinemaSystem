using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CinemaSystemWebapp.Utils
{
    public static class Crypto
    {
        public static string Base64Encode(byte[] plainTextBytes)
        {
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return Base64Encode(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static string HashHMAC(string key, string message)
        {
            var keyBytes = System.Text.Encoding.UTF8.GetBytes(key);
            var messageBytes = System.Text.Encoding.UTF8.GetBytes(message);

            var hash = new HMACSHA256(keyBytes);
            return Base64Encode(hash.ComputeHash(messageBytes));
        }

        public static string SHA256(string randomString)
        {
            var crypt = System.Security.Cryptography.SHA256.Create();
            string hash = String.Empty;
            byte[] crypto = crypt.ComputeHash(Encoding.ASCII.GetBytes(randomString));
            foreach (byte theByte in crypto)
            {
                hash += theByte.ToString("x2");
            }
            return hash;
        }

        public static string MD5(Stream stream)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            using (MemoryStream _stream = new MemoryStream())
            {
                stream.Seek(0, SeekOrigin.Begin);
                stream.CopyTo(_stream);

                string hash = String.Empty;
                foreach (byte theByte in md5.ComputeHash(_stream))
                {
                    hash += theByte.ToString("x2");
                }

                return hash;
            }
        }

        const int BYTES_TO_READ = sizeof(Int64);
        public static bool FilesAreEqual(Stream first, Stream second)
        {
            using (MemoryStream fs1 = new MemoryStream())
            using (MemoryStream fs2 = new MemoryStream())
            {
                first.Seek(0, SeekOrigin.Begin);
                second.Seek(0, SeekOrigin.Begin);

                first.CopyTo(fs1);
                second.CopyTo(fs2);

                if (first.Length != second.Length)
                    return false;

                int iterations = (int)Math.Ceiling((double)first.Length / BYTES_TO_READ);

                byte[] one = new byte[BYTES_TO_READ];
                byte[] two = new byte[BYTES_TO_READ];

                for (int i = 0; i < iterations; i++)
                {
                    fs1.Read(one, 0, BYTES_TO_READ);
                    fs2.Read(two, 0, BYTES_TO_READ);

                    if (BitConverter.ToInt64(one, 0) != BitConverter.ToInt64(two, 0))
                        return false;
                }
            }

            return true;
        }
    }
}
