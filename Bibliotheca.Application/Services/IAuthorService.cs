using Bibliotheca.Application.Dtos.Author;
using Bibliotheca.Application.Dtos.Common;

namespace Bibliotheca.Application.Services;

public interface IAuthorService
{
    Task<ResponseDto<AuthorDto>> GetByIdAsync(Guid id);

    Task<ResponseDto<List<AuthorDto>>> GetAllAsync();

    Task<ResponseDto<AuthorDto>> CreateAsync(CreateAuthorDto dto);

    Task<ResponseDto<AuthorDto>> UpdateAsync(UpdateAuthorDto dto);

    Task<ResponseDto<bool>> DeleteAsync(Guid id);
}