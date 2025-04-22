namespace MVC_Project.Services.AuthServices
{
    using System.Security.Cryptography;
    using System.Text;

    public static class Sha256Hasher
    {
        public static string GenerateSalt(int size = 16)
        {
            byte[] saltBytes = new byte[size];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        public static string HashPassword(string password, string salt)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                string saltedPassword = password + salt;
                byte[] bytes = Encoding.UTF8.GetBytes(saltedPassword);
                byte[] hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        public static bool VerifyPassword(string enteredPassword, string salt, string storedHash)
        {
            string hashOfInput = HashPassword(enteredPassword, salt);
            return hashOfInput == storedHash;
        }
    }
}
