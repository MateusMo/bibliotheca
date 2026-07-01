// Bibliotheca.Application/Dtos/Profile/UpdateProfileDto.cs
namespace Bibliotheca.Application.Dtos.Profile;

public class UpdateProfileDto
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Contact { get; set; } = string.Empty;
}