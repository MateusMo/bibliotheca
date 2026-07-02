using Bibliotheca.Application.Dtos.Book;
using Bibliotheca.Application.Dtos.Common;

namespace Bibliotheca.Application.Services;

public interface IBookService
{
    Task<ResponseDto<BookDto>> GetByIdAsync(Guid id);
    Task<ResponseDto<List<BookDto>>> GetAllAsync();
    Task<ResponseDto<List<BookDto>>> GetByUserIdAsync(Guid userId);
    Task<ResponseDto<List<BookDto>>> GetByAuthorIdAsync(Guid authorId);
    Task<ResponseDto<PagedResultDto<BookDto>>> SearchAsync(BookFilterDto filter, int pageNumber, int pageSize);
    Task<ResponseDto<PagedResultDto<BookDto>>> GetByUserIdPagedAsync(Guid userId, int pageNumber, int pageSize);
    Task<ResponseDto<BookDto>> CreateAsync(CreateBookDto dto);
    Task<ResponseDto<BookDto>> UpdateAsync(UpdateBookDto dto);
    Task<ResponseDto<bool>> DeleteAsync(Guid id);
}