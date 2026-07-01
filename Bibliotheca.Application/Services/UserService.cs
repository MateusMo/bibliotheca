// Bibliotheca.Application/Services/UserService.cs
using Bibliotheca.Application.Dtos.Common;
using Bibliotheca.Application.Dtos.User;
using Bibliotheca.Application.InfraServices;
using Bibliotheca.Data.Uow;
using Bibliotheca.Domain.Domains;

namespace Bibliotheca.Application.Services;

public class UserService : BaseService, IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICrypto _crypto;

    public UserService(IUnitOfWork unitOfWork, ICrypto crypto)
    {
        _unitOfWork = unitOfWork;
        _crypto = crypto;
    }

    public async Task<ResponseDto<UserDto>> GetByIdAsync(Guid id)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);

        if (user is null || !user.IsActive)
            return Failure<UserDto>("User not found", 404);

        return Success(ToDto(user));
    }

    public async Task<ResponseDto<List<UserDto>>> GetAllAsync()
    {
        var users = await _unitOfWork.Users.FindAllAsync(u => u.IsActive);
        return Success(users.Select(ToDto).ToList());
    }

    public async Task<ResponseDto<UserDto>> GetByEmailAsync(string email)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(email);

        if (user is null || !user.IsActive)
            return Failure<UserDto>("User not found", 404);

        return Success(ToDto(user));
    }

    public async Task<ResponseDto<UserDto>> CreateAsync(CreateUserDto dto)
    {
        var emailExists = await _unitOfWork.Users.EmailExistsAsync(dto.Email);
        if (emailExists)
            return Failure<UserDto>("Email already in use", 409);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Email = dto.Email,
            Password = _crypto.HashPassword(dto.Password),
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };
        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return Success(ToDto(user), "User created successfully", 201);
    }

    public async Task<ResponseDto<UserDto>> UpdateAsync(UpdateUserDto dto)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(dto.Id);

        if (user is null || !user.IsActive)
            return Failure<UserDto>("User not found", 404);

        if (!string.Equals(user.Email, dto.Email, StringComparison.OrdinalIgnoreCase))
        {
            var emailExists = await _unitOfWork.Users.EmailExistsAsync(dto.Email);
            if (emailExists)
                return Failure<UserDto>("Email already in use", 409);
        }

        user.Name = dto.Name;
        user.Email = dto.Email;
        user.UpdatedAt = DateTimeOffset.UtcNow;

        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return Success(ToDto(user), "User updated successfully");
    }

    public async Task<ResponseDto<bool>> DeleteAsync(Guid id)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);

        if (user is null || !user.IsActive)
            return Failure<bool>("User not found", 404);

        user.IsActive = false;
        user.UpdatedAt = DateTimeOffset.UtcNow;

        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return Success(true, "User removed successfully");
    }

    private static UserDto ToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            Name = user.Name,
            Email = user.Email
        };
    }
}