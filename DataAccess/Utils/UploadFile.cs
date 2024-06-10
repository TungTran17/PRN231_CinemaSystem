namespace DataAccess.Utils
{
    public static class UploadFile
    {
        public static (string FilePath, string FileName) Upload(string uploadPath, string filename, Stream stream)
        {
            // Ensure the directory exists
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            string hashedFileName = Crypto.MD5(stream) + Path.GetExtension(filename);

            string filepath = Path.Combine(uploadPath, $"_0_{hashedFileName}");
            int index = 0;
            for (; File.Exists(filepath); index++, filepath = Path.Combine(uploadPath, $"_{index}_{hashedFileName}"))
            {
                using (var fs1 = new FileStream(filepath, FileMode.Open, FileAccess.Read))
                {
                    using (var fs2 = new MemoryStream())
                    {
                        stream.Position = 0; // Reset stream position
                        stream.CopyTo(fs2); // Copy stream to memory stream for comparison
                        fs2.Position = 0; // Reset memory stream position
                        if (Crypto.FilesAreEqual(fs1, fs2))
                        {
                            return (filepath, $"_{index}_{hashedFileName}");
                        }
                    }
                }
            }

            stream.Position = 0; // Reset stream position before writing to file
            using (var fileStream = new FileStream(filepath, FileMode.Create))
            {
                stream.CopyTo(fileStream);
            }

            return (filepath, $"_{index}_{hashedFileName}");
        }

    }
}