using Bibliotheca.Application.Dtos.Comment;
using Bibliotheca.Application.Dtos.Common;
using Bibliotheca.Data.Uow;
using Bibliotheca.Domain.Domains;

namespace Bibliotheca.Application.Services;

public class CommentService : BaseService, ICommentService
{
    private readonly IUnitOfWork _unitOfWork;

    public CommentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseDto<CommentDto>> GetByIdAsync(Guid id)
    {
        var comment = await _unitOfWork.Comments.GetByIdAsync(id);

        if (comment is null || !comment.IsActive)
            return Failure<CommentDto>("Comment not found", 404);

        return Success(ToDto(comment));
    }

    public async Task<ResponseDto<List<CommentDto>>> GetAllAsync()
    {
        var comments = await _unitOfWork.Comments.FindAllAsync(c => c.IsActive);
        return Success(comments.Select(ToDto).ToList());
    }

    public async Task<ResponseDto<List<CommentDto>>> GetByUserIdAsync(Guid userId)
    {
        var comments = await _unitOfWork.Comments.GetByUserIdAsync(userId);
        return Success(comments.Where(c => c.IsActive).Select(ToDto).ToList());
    }

    public async Task<ResponseDto<List<CommentDto>>> GetByBookIdAsync(Guid bookId)
    {
        var comments = await _unitOfWork.Comments.GetByBookIdAsync(bookId);
        return Success(comments.Where(c => c.IsActive).Select(ToDto).ToList());
    }

    public async Task<ResponseDto<CommentDto>> CreateAsync(CreateCommentDto dto)
    {
        var userExists = await _unitOfWork.Users.ExistsAsync(u => u.Id == dto.UserId && u.IsActive);
        if (!userExists)
            return Failure<CommentDto>("User not found", 404);

        var bookExists = await _unitOfWork.Books.ExistsAsync(b => b.Id == dto.BookId && b.IsActive);
        if (!bookExists)
            return Failure<CommentDto>("Book not found", 404);

        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            UserId = dto.UserId,
            BookId = dto.BookId,
            Content = dto.Content,
            Link = dto.Link,
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        await _unitOfWork.Comments.AddAsync(comment);
        await _unitOfWork.SaveChangesAsync();

        return Success(ToDto(comment), "Comment created successfully", 201);
    }

    public async Task<ResponseDto<CommentDto>> UpdateAsync(UpdateCommentDto dto)
    {
        var comment = await _unitOfWork.Comments.GetByIdAsync(dto.Id);

        if (comment is null || !comment.IsActive)
            return Failure<CommentDto>("Comment not found", 404);

        comment.Content = dto.Content;
        comment.Link = dto.Link;
        comment.UpdatedAt = DateTimeOffset.UtcNow;

        _unitOfWork.Comments.Update(comment);
        await _unitOfWork.SaveChangesAsync();

        return Success(ToDto(comment), "Comment updated successfully");
    }

    public async Task<ResponseDto<bool>> DeleteAsync(Guid id)
    {
        var comment = await _unitOfWork.Comments.GetByIdAsync(id);

        if (comment is null || !comment.IsActive)
            return Failure<bool>("Comment not found", 404);

        comment.IsActive = false;
        comment.UpdatedAt = DateTimeOffset.UtcNow;

        _unitOfWork.Comments.Update(comment);
        await _unitOfWork.SaveChangesAsync();

        return Success(true, "Comment removed successfully");
    }

    private static CommentDto ToDto(Comment comment)
    {
        return new CommentDto
        {
            Id = comment.Id,
            CreatedAt = comment.CreatedAt,
            UpdatedAt = comment.UpdatedAt,
            UserId = comment.UserId,
            UserName = comment.User?.Name ?? string.Empty,
            BookId = comment.BookId,
            Content = comment.Content,
            Link = comment.Link
        };
    }
}