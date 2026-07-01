// Bibliotheca.Application/Dtos/User/UserDto.cs
using Bibliotheca.Application.Dtos.Common;

namespace Bibliotheca.Application.Dtos.User;

public class UserDto : BaseDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    // Password NUNCA sai daqui de propósito.
}