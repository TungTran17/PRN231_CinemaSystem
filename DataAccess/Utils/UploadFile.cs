
using System.Collections.Generic;

namespace CinemaSystemWebapp.Utils
{
    public static class UploadFile
    {
        public static (string FilePath, string FileName) Upload(string uploadPath, string filename, Stream stream)
        {
            string hashedFileName = Crypto.MD5(stream) + Path.GetExtension(filename);

            string filepath = Path.Combine(uploadPath, $"_0_{hashedFileName}");
            int index = 0;
            for (;
                File.Exists(filepath);
                index++, filepath = Path.Combine(uploadPath, $"_{index}_{hashedFileName}"))
            {
                var fs1 = new FileStream(filepath, FileMode.Open, FileAccess.Read);
                var fs2 = stream;
                if (Crypto.FilesAreEqual(fs1, fs2))
                {
                    return (filepath, $"_{index}_{hashedFileName}");
                }
            }

            byte[] data = new byte[stream.Length];
            stream.Seek(0, SeekOrigin.Begin);
            if (stream.Read(data) == stream.Length)
                File.WriteAllBytes(filepath, data);

            return (filepath, $"_{index}_{hashedFileName}");
        }
    }
}
