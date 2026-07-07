// Bibliotheca.Application/Services/ProfileService.cs
using Bibliotheca.Application.Dtos.Common;
using Bibliotheca.Application.Dtos.Profile;
using Bibliotheca.Data.Uow;
using Bibliotheca.Domain.Domains;

namespace Bibliotheca.Application.Services;

public class ProfileService : BaseService, IProfileService
{
    private readonly IUnitOfWork _unitOfWork;
    public ProfileService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseDto<ProfileDto>> GetByIdAsync(Guid id)
    {
        var profile = await _unitOfWork.Profiles.GetByIdAsync(id);

        if (profile is null || !profile.IsActive)
            return Failure<ProfileDto>("Profile not found", 404);

        return Success(ToDto(profile));
    }

    public async Task<ResponseDto<ProfileDto>> GetByUserIdAsync(Guid userId)
    {
        var profile = await _unitOfWork.Profiles.GetByUserIdAsync(userId);

        if (profile is null || !profile.IsActive)
            return Failure<ProfileDto>("Profile not found", 404);

        return Success(ToDto(profile));
    }

    public async Task<ResponseDto<ProfileDto>> CreateAsync(CreateProfileDto dto)
    {
        var userExists = await _unitOfWork.Users.ExistsAsync(u => u.Id == dto.UserId && u.IsActive);
        if (!userExists)
            return Failure<ProfileDto>("User not found", 404);

        var existingProfile = await _unitOfWork.Profiles.GetByUserIdAsync(dto.UserId);
        if (existingProfile is not null && existingProfile.IsActive)
            return Failure<ProfileDto>("User already has a profile", 409);

        var profile = new Profile
        {
            Id = Guid.NewGuid(),
            UserId = dto.UserId,
            Description = dto.Description,
            Contact = dto.Contact,
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        await _unitOfWork.Profiles.AddAsync(profile);
        await _unitOfWork.SaveChangesAsync();

        return Success(ToDto(profile), "Profile created successfully", 201);
    }

    public async Task<ResponseDto<ProfileDto>> UpdateAsync(UpdateProfileDto dto)
    {
        var profile = await _unitOfWork.Profiles.GetByIdAsync(dto.Id);

        if (profile is null || !profile.IsActive)
            return Failure<ProfileDto>("Profile not found", 404);

        profile.Description = dto.Description;
        profile.Contact = dto.Contact;
        profile.UpdatedAt = DateTimeOffset.UtcNow;

        _unitOfWork.Profiles.Update(profile);
        await _unitOfWork.SaveChangesAsync();

        return Success(ToDto(profile), "Profile updated successfully");
    }

    public async Task<ResponseDto<bool>> DeleteAsync(Guid id)
    {
        var profile = await _unitOfWork.Profiles.GetByIdAsync(id);

        if (profile is null || !profile.IsActive)
            return Failure<bool>("Profile not found", 404);

        profile.IsActive = false;
        profile.UpdatedAt = DateTimeOffset.UtcNow;

        _unitOfWork.Profiles.Update(profile);
        await _unitOfWork.SaveChangesAsync();

        return Success(true, "Profile removed successfully");
    }

    private static ProfileDto ToDto(Profile profile)
    {
        return new ProfileDto
        {
            Id = profile.Id,
            CreatedAt = profile.CreatedAt,
            UpdatedAt = profile.UpdatedAt,
            UserId = profile.UserId,
            Description = profile.Description,
            Contact = profile.Contact
        };
    }
}