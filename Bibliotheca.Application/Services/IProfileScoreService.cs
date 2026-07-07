using Bibliotheca.Application.Dtos.Common;
using Bibliotheca.Application.Dtos.ProfileScore;

namespace Bibliotheca.Application.Services;

public interface IProfileScoreService
{
    Task<ResponseDto<ProfileScoreDto>> GetByUserIdAsync(Guid userId);
    Task<ResponseDto<bool>> RecalculateAsync(Guid userId);
}