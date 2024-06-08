namespace DataAccess.Utils
{
    public static class StringUtils
    {
        public static bool CheckPasswordValidate(string? password)
        {
            string regexPattern = "^(?=.*[A-Z])(?=.*\\d)[A-Za-z0-9]{6,}$";
            return System.Text.RegularExpressions.Regex.IsMatch(password, regexPattern);
        }

    }
}
