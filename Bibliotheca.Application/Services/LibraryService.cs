using Bibliotheca.Application.Dtos.Common;
using Bibliotheca.Application.Dtos.Library;
using Bibliotheca.Data.Uow;
using Bibliotheca.Domain.Domains;

namespace Bibliotheca.Application.Services;

public class LibraryService : BaseService, ILibraryService
{
    private readonly IUnitOfWork _unitOfWork;

    public LibraryService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseDto<LibraryDto>> GetByIdAsync(Guid id)
    {
        var library = await _unitOfWork.Libraries.GetByIdWithBooksAsync(id);

        if (library is null || !library.IsActive)
            return Failure<LibraryDto>("Library not found", 404);

        return Success(ToDto(library));
    }

    public async Task<ResponseDto<PagedResultDto<LibraryDto>>> GetByUserIdPagedAsync(Guid userId, int pageNumber, int pageSize)
    {
        var paged = await _unitOfWork.Libraries.GetByUserIdPagedAsync(userId, pageNumber, pageSize);

        var dto = new PagedResultDto<LibraryDto>
        {
            Items = paged.Items.Select(ToDto).ToList(),
            PageNumber = paged.PageNumber,
            PageSize = paged.PageSize,
            TotalCount = paged.TotalCount
        };

        return Success(dto);
    }

    public async Task<ResponseDto<LibraryDto>> CreateAsync(CreateLibraryDto dto)
    {
        var userExists = await _unitOfWork.Users.ExistsAsync(u => u.Id == dto.UserId && u.IsActive);
        if (!userExists)
            return Failure<LibraryDto>("User not found", 404);

        // Só entram na library os livros que realmente pertencem ao usuário —
        // ids de terceiros enviados pelo cliente são descartados silenciosamente aqui.
        var books = await _unitOfWork.Books.GetTrackedByIdsForUserAsync(dto.UserId, dto.BookIds);

        var library = new Library
        {
            Id = Guid.NewGuid(),
            UserId = dto.UserId,
            Title = dto.Title,
            Description = dto.Description,
            Books = books,
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        await _unitOfWork.Libraries.AddAsync(library);
        await _unitOfWork.SaveChangesAsync();

        return Success(ToDto(library), "Library created successfully", 201);
    }

    public async Task<ResponseDto<LibraryDto>> UpdateAsync(UpdateLibraryDto dto)
    {
        var library = await _unitOfWork.Libraries.GetByIdTrackedWithBooksAsync(dto.Id);

        if (library is null || !library.IsActive)
            return Failure<LibraryDto>("Library not found", 404);

        var books = await _unitOfWork.Books.GetTrackedByIdsForUserAsync(library.UserId, dto.BookIds);

        library.Title = dto.Title;
        library.Description = dto.Description;
        library.Books.Clear();
        foreach (var book in books)
            library.Books.Add(book);
        library.UpdatedAt = DateTimeOffset.UtcNow;

        _unitOfWork.Libraries.Update(library);
        await _unitOfWork.SaveChangesAsync();

        return Success(ToDto(library), "Library updated successfully");
    }

    public async Task<ResponseDto<bool>> DeleteAsync(Guid id, Guid requestingUserId)
    {
        var library = await _unitOfWork.Libraries.GetByIdAsync(id);

        if (library is null || !library.IsActive)
            return Failure<bool>("Library not found", 404);

        if (library.UserId != requestingUserId)
            return Failure<bool>("You are not allowed to remove this library", 403);

        library.IsActive = false;
        library.UpdatedAt = DateTimeOffset.UtcNow;

        _unitOfWork.Libraries.Update(library);
        await _unitOfWork.SaveChangesAsync();

        return Success(true, "Library removed successfully");
    }

    private static LibraryDto ToDto(Library library)
    {
        return new LibraryDto
        {
            Id = library.Id,
            CreatedAt = library.CreatedAt,
            UpdatedAt = library.UpdatedAt,
            UserId = library.UserId,
            UserName = library.User?.Name ?? string.Empty,
            ProfileScore = library.User?.Profile?.ProfileScore?.TotalScore ?? 0,
            Title = library.Title,
            Description = library.Description,
            Books = library.Books.Select(b => new LibraryBookItemDto
            {
                Id = b.Id,
                Name = b.Name,
                Author = b.Author,
                PublicationYear = b.PublicationYear,
                Photos = b.Photos,
                LanguageEnum = b.LanguageEnum,
                ConditionEnum = b.ConditionEnum,
                ViewCount = b.ViewCount,
                CreatedAt = b.CreatedAt
            }).ToList()
        };
    }
}