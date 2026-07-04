// Bibliotheca.Application/Services/ICommentService.cs
using Bibliotheca.Application.Dtos.Comment;
using Bibliotheca.Application.Dtos.Common;

namespace Bibliotheca.Application.Services;

public interface ICommentService
{
    Task<ResponseDto<CommentDto>> GetByIdAsync(Guid id);
    Task<ResponseDto<List<CommentDto>>> GetAllAsync();
    Task<ResponseDto<List<CommentDto>>> GetByUserIdAsync(Guid userId);
    Task<ResponseDto<List<CommentDto>>> GetByBookIdAsync(Guid bookId);

    Task<ResponseDto<PagedResultDto<CommentDto>>> GetByUserIdPagedAsync(Guid userId, int pageNumber, int pageSize);
    Task<ResponseDto<PagedResultDto<CommentDto>>> GetByBookIdPagedAsync(Guid bookId, int pageNumber, int pageSize);

    Task<ResponseDto<CommentDto>> CreateAsync(CreateCommentDto dto);
    Task<ResponseDto<CommentDto>> UpdateAsync(UpdateCommentDto dto);
    Task<ResponseDto<bool>> DeleteAsync(Guid id);
}