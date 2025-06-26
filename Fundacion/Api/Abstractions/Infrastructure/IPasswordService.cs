namespace Api.Abstractions.Infrastructure
{
    public interface IPasswordService
    {
        string GeneratePassword(int length);
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);
    }
}