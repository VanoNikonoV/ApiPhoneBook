using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ApiPhoneBook.Models
{
    public class AuthOptions
    {
        public const string ISSUER = "ApiPhoneBook"; // издатель токена
        public const string AUDIENCE = "PhoneBook"; // потребитель токена
        const string KEY = "My top secret Key №1";   // ключ для шифрации
        public const int LIFETIME = 10; // время жизни токена - 10 минута
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
        }
    }
}
