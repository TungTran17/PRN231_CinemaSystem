namespace DataAccess.Utils
{
    public static class StringUtils
    {
        public class PasswordValidationResult
        {
            public bool IsValid { get; set; }
            public List<string> ErrorMessages { get; set; } = new List<string>();
        }

        public static PasswordValidationResult CheckPasswordValidate(string? password)
        {
            var result = new PasswordValidationResult();

            if (string.IsNullOrEmpty(password))
            {
                result.IsValid = false;
                result.ErrorMessages.Add("Password cannot be empty.");
                return result;
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(password, "^(?=.*[A-Z])"))
            {
                result.IsValid = false;
                result.ErrorMessages.Add("Password must contain at least one uppercase letter.");
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(password, "(?=.*\\d)"))
            {
                result.IsValid = false;
                result.ErrorMessages.Add("Password must contain at least one digit.");
            }

            if (password.Length < 6)
            {
                result.IsValid = false;
                result.ErrorMessages.Add("Password must be at least 6 characters long.");
            }

            result.IsValid = result.ErrorMessages.Count == 0;
            return result;
        }
    }

}
