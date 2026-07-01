// Bibliotheca.Application/Dtos/User/CreateUserDto.cs
namespace Bibliotheca.Application.Dtos.User;

public class CreateUserDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty; // texto puro, hash acontece no service
}