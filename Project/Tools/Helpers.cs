using System.Security.Cryptography;
using System.Text;

namespace Project.Tools
{
    class Helpers
    {
        public string HashPassword(string password) 
        {
            using (var sha256 = SHA256.Create())
            {
                // Генерация соли
                byte[] salt = Encoding.UTF8.GetBytes("ikv1980MadK");
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] saltedPassword = passwordBytes.Concat(salt).ToArray();

                // Хэширование
                byte[] hash = sha256.ComputeHash(saltedPassword);
                return Convert.ToBase64String(hash);
            }
        }
    }
}
