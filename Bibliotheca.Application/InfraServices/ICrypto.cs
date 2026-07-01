// Bibliotheca.Application/InfraServices/ICrypto.cs
namespace Bibliotheca.Application.InfraServices;

public interface ICrypto
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hashedPassword);
}