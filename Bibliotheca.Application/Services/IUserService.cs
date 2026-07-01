// Bibliotheca.Application/Services/IUserService.cs
using Bibliotheca.Application.Dtos.Common;
using Bibliotheca.Application.Dtos.User;

namespace Bibliotheca.Application.Services;

public interface IUserService
{
    Task<ResponseDto<UserDto>> GetByIdAsync(Guid id);
    Task<ResponseDto<List<UserDto>>> GetAllAsync();
    Task<ResponseDto<UserDto>> GetByEmailAsync(string email);
    Task<ResponseDto<UserDto>> CreateAsync(CreateUserDto dto);
    Task<ResponseDto<UserDto>> UpdateAsync(UpdateUserDto dto);
    Task<ResponseDto<bool>> DeleteAsync(Guid id);
}