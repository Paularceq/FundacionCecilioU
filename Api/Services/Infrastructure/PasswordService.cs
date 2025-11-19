using Api.Abstractions.Infrastructure;
using System.Security.Cryptography;

namespace Api.Services.Infrastructure
{
    public class PasswordService : IPasswordService
    {
        private const string ValidChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()_-+=<>?";

        public string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Password cannot be null or empty.", nameof(password));
            }
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hashedPassword))
            {
                throw new ArgumentException("Password and hashed password cannot be null or empty.");
            }
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }

        public string GeneratePassword(int length)
        {
            if (length <= 0)
                throw new ArgumentOutOfRangeException(nameof(length), "Password length must be greater than 0.");

            return string.Create(length, ValidChars, (span, valid) =>
            {
                using var rng = RandomNumberGenerator.Create();
                var buffer = new byte[sizeof(uint)];

                for (int i = 0; i < span.Length; i++)
                {
                    rng.GetBytes(buffer);
                    uint num = BitConverter.ToUInt32(buffer, 0);
                    span[i] = valid[(int)(num % (uint)valid.Length)];
                }
            });
        }
    }
}
