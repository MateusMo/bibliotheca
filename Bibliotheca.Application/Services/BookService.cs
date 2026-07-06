using Bibliotheca.Application.Dtos.Book;
using Bibliotheca.Application.Dtos.Common;
using Bibliotheca.Data.Repositories.Dto;
using Bibliotheca.Data.Uow;
using Bibliotheca.Domain.Domains;

namespace Bibliotheca.Application.Services;

public class BookService : BaseService, IBookService
{
    private readonly IUnitOfWork _unitOfWork;

    public BookService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseDto<BookDto>> GetByIdAsync(Guid id)
    {
        var book = await _unitOfWork.Books.GetByIdAsync(id);

        if (book is null || !book.IsActive)
            return Failure<BookDto>("Book not found", 404);

        return Success(ToDto(book));
    }

    public async Task<ResponseDto<List<BookDto>>> GetAllAsync()
    {
        var books = await _unitOfWork.Books.GetAllAsync();
        return Success(books.Where(b => b.IsActive).Select(ToDto).ToList());
    }

    public async Task<ResponseDto<List<BookDto>>> GetByUserIdAsync(Guid userId)
    {
        var books = await _unitOfWork.Books.GetByUserIdAsync(userId);
        return Success(books.Where(b => b.IsActive).Select(ToDto).ToList());
    }
    
    public async Task<ResponseDto<bool>> RegisterViewAsync(Guid id)
    {
        var exists = await _unitOfWork.Books.ExistsAsync(b => b.Id == id && b.IsActive);
        if (!exists)
            return Failure<bool>("Book not found", 404);

        await _unitOfWork.Books.IncrementViewCountAsync(id);

        return Success(true);
    }
    
    public async Task<ResponseDto<PagedResultDto<BookDto>>> SearchAsync(BookFilterDto filter, int pageNumber, int pageSize)
    {
        var searchFilter = new BookSearchFilter
        {
            SearchText = filter.SearchText,
            YearFrom = filter.YearFrom,
            YearTo = filter.YearTo,
            AddedFrom = filter.AddedFrom,
            AddedTo = filter.AddedTo,
            PagesFrom = filter.PagesFrom,
            PagesTo = filter.PagesTo,
            Language = filter.Language,
            Condition = filter.Condition,
            ValueFrom = filter.ValueFrom,
            ValueTo = filter.ValueTo,
            SortBy = filter.SortBy
        };

        var paged = await _unitOfWork.Books.SearchPagedAsync(searchFilter, pageNumber, pageSize);

        var dto = new PagedResultDto<BookDto>
        {
            Items = paged.Items.Select(ToDto).ToList(),
            PageNumber = paged.PageNumber,
            PageSize = paged.PageSize,
            TotalCount = paged.TotalCount
        };

        return Success(dto);
    }

    public async Task<ResponseDto<PagedResultDto<BookDto>>> GetByUserIdPagedAsync(Guid userId, int pageNumber, int pageSize)
    {
        var paged = await _unitOfWork.Books.GetByUserIdPagedAsync(userId, pageNumber, pageSize);

        var dto = new PagedResultDto<BookDto>
        {
            Items = paged.Items.Select(ToDto).ToList(),
            PageNumber = paged.PageNumber,
            PageSize = paged.PageSize,
            TotalCount = paged.TotalCount
        };

        return Success(dto);
    }

    public async Task<ResponseDto<BookDto>> CreateAsync(CreateBookDto dto)
    {
        var book = new Book
        {
            Id = Guid.NewGuid(),
            UserId = dto.UserId,
            IsOwner = dto.IsOwner,
            Name = dto.Name,
            Author = dto.Author,
            Description = dto.Description,
            PublicationYear = dto.PublicationYear,
            Photos = dto.Photos,
            LanguageEnum = dto.LanguageEnum,
            Publisher = dto.Publisher,
            ISBN = dto.ISBN,
            Pages = dto.Pages,
            EstimatedValue = dto.EstimatedValue,
            ConditionEnum = dto.ConditionEnum,
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        await _unitOfWork.Books.AddAsync(book);
        await _unitOfWork.SaveChangesAsync();

        return Success(ToDto(book), "Book created successfully", 201);
    }

    public async Task<ResponseDto<BookDto>> UpdateAsync(UpdateBookDto dto)
    {
        var book = await _unitOfWork.Books.GetByIdAsync(dto.Id);

        if (book is null || !book.IsActive)
            return Failure<BookDto>("Book not found", 404);

        book.IsOwner = dto.IsOwner;
        book.Name = dto.Name;
        book.Author = dto.Author;
        book.Description = dto.Description;
        book.PublicationYear = dto.PublicationYear;
        book.Photos = dto.Photos;
        book.LanguageEnum = dto.LanguageEnum;
        book.Publisher = dto.Publisher;
        book.ISBN = dto.ISBN;
        book.Pages = dto.Pages;
        book.EstimatedValue = dto.EstimatedValue;
        book.ConditionEnum = dto.ConditionEnum;
        book.UpdatedAt = DateTimeOffset.UtcNow;

        _unitOfWork.Books.Update(book);
        await _unitOfWork.SaveChangesAsync();

        return Success(ToDto(book), "Book updated successfully");
    }

    public async Task<ResponseDto<bool>> DeleteAsync(Guid id)
    {
        var book = await _unitOfWork.Books.GetByIdAsync(id);

        if (book is null || !book.IsActive)
            return Failure<bool>("Book not found", 404);

        book.IsActive = false;
        book.UpdatedAt = DateTimeOffset.UtcNow;

        _unitOfWork.Books.Update(book);
        await _unitOfWork.SaveChangesAsync();

        return Success(true, "Book removed successfully");
    }

    private static BookDto ToDto(Book book)
    {
        return new BookDto
        {
            Id = book.Id,
            CreatedAt = book.CreatedAt,
            UpdatedAt = book.UpdatedAt,
            UserId = book.UserId,
            IsOwner = book.IsOwner,
            Name = book.Name,
            Author = book.Author,
            Description = book.Description,
            PublicationYear = book.PublicationYear,
            Photos = book.Photos,
            LanguageEnum = book.LanguageEnum,
            Publisher = book.Publisher,
            ISBN = book.ISBN,
            Pages = book.Pages,
            EstimatedValue = book.EstimatedValue,
            ConditionEnum = book.ConditionEnum
        };
    }
}