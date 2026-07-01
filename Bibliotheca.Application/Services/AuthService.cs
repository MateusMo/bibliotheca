using Bibliotheca.Application.Dtos.Auth;
using Bibliotheca.Application.Dtos.Common;
using Bibliotheca.Application.Dtos.User;
using Bibliotheca.Application.InfraServices;
using Bibliotheca.Data.Uow;

namespace Bibliotheca.Application.Services;

public class AuthService : BaseService, IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICrypto _crypto;
    private readonly IUserService _userService;

    public AuthService(IUnitOfWork unitOfWork, ICrypto crypto, IUserService userService)
    {
        _unitOfWork = unitOfWork;
        _crypto = crypto;
        _userService = userService;
    }

    public async Task<ResponseDto<UserDto>> LoginAsync(LoginDto dto)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(dto.Email);

        if (user is null || !user.IsActive || !_crypto.VerifyPassword(dto.Password, user.Password))
            return Failure<UserDto>("Invalid email or password", 401);

        return Success(new UserDto
        {
            Id = user.Id,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            Name = user.Name,
            Email = user.Email
        });
    }

    public Task<ResponseDto<UserDto>> RegisterAsync(CreateUserDto dto)
        => _userService.CreateAsync(dto);
}