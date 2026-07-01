// Bibliotheca.Application/Dtos/User/UpdateUserDto.cs
namespace Bibliotheca.Application.Dtos.User;

public class UpdateUserDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    // Troca de senha deveria ser um endpoint/DTO separado
    // (ex: ChangePasswordDto com senha atual + nova), não esse Update genérico.
}