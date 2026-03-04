using System.Security.Cryptography;
using System.Text;

namespace FAP_API.Services
{
    public class HashPassword
    {
        public string HashToSha256(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);
                return Convert.ToHexString(hashBytes);
            }
        }
    }
}
