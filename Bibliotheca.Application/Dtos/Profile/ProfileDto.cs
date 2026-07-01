// Bibliotheca.Application/Dtos/Profile/ProfileDto.cs
using Bibliotheca.Application.Dtos.Common;

namespace Bibliotheca.Application.Dtos.Profile;

public class ProfileDto : BaseDto
{
    public Guid UserId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Contact { get; set; } = string.Empty;
}