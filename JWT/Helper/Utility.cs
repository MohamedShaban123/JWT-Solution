using System.Security.Cryptography;

namespace JWT.Helper
{
    public static class Utility
    {
        public static string GenerateRandomString()
        {
            var randomBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }

            return Convert.ToBase64String(randomBytes);
        }
    }
}
