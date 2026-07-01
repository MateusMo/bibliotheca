// Bibliotheca.Application/Services/IProfileService.cs
using Bibliotheca.Application.Dtos.Common;
using Bibliotheca.Application.Dtos.Profile;

namespace Bibliotheca.Application.Services;

public interface IProfileService
{
    Task<ResponseDto<ProfileDto>> GetByIdAsync(Guid id);
    Task<ResponseDto<ProfileDto>> GetByUserIdAsync(Guid userId);
    Task<ResponseDto<ProfileDto>> CreateAsync(CreateProfileDto dto);
    Task<ResponseDto<ProfileDto>> UpdateAsync(UpdateProfileDto dto);
    Task<ResponseDto<bool>> DeleteAsync(Guid id);
}