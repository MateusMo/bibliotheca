// Bibliotheca.Application/InfraServices/Crypto.cs
namespace Bibliotheca.Application.InfraServices;

public class Crypto : ICrypto
{
    public string HashPassword(string password)
        => BCrypt.Net.BCrypt.HashPassword(password);

    public bool VerifyPassword(string password, string hashedPassword)
        => BCrypt.Net.BCrypt.Verify(password, hashedPassword);
}