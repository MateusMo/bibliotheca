// Bibliotheca.Application/Dtos/Profile/CreateProfileDto.cs
namespace Bibliotheca.Application.Dtos.Profile;

public class CreateProfileDto
{
    public Guid UserId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Contact { get; set; } = string.Empty;
}