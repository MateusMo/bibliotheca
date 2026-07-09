using Bibliotheca.Application.Dtos.Common;
using Bibliotheca.Application.Dtos.Library;

namespace Bibliotheca.Application.Services;

public interface ILibraryService
{
    Task<ResponseDto<LibraryDto>> GetByIdAsync(Guid id);
    Task<ResponseDto<PagedResultDto<LibraryDto>>> GetByUserIdPagedAsync(Guid userId, int pageNumber, int pageSize);
    Task<ResponseDto<LibraryDto>> CreateAsync(CreateLibraryDto dto);
    Task<ResponseDto<LibraryDto>> UpdateAsync(UpdateLibraryDto dto);
    Task<ResponseDto<bool>> DeleteAsync(Guid id, Guid requestingUserId);
}