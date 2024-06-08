using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
