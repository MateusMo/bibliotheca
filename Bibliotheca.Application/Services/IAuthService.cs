using Bibliotheca.Application.Dtos.Auth;
using Bibliotheca.Application.Dtos.Common;
using Bibliotheca.Application.Dtos.User;

namespace Bibliotheca.Application.Services;

public interface IAuthService
{
    Task<ResponseDto<UserDto>> LoginAsync(LoginDto dto);
    Task<ResponseDto<UserDto>> RegisterAsync(CreateUserDto dto);
}