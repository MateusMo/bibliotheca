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
        var book = await _unitOfWork.Books.GetByIdWithAuthorsAsync(id);

        if (book is null || !book.IsActive)
            return Failure<BookDto>("Book not found", 404);

        return Success(ToDto(book));
    }

    public async Task<ResponseDto<List<BookDto>>> GetAllAsync()
    {
        var books = await _unitOfWork.Books.GetAllWithAuthorsAsync();
        return Success(books.Where(b => b.IsActive).Select(ToDto).ToList());
    }

    public async Task<ResponseDto<List<BookDto>>> GetByUserIdAsync(Guid userId)
    {
        var books = await _unitOfWork.Books.GetByUserIdAsync(userId);
        return Success(books.Where(b => b.IsActive).Select(ToDto).ToList());
    }

    public async Task<ResponseDto<List<BookDto>>> GetByAuthorIdAsync(Guid authorId)
    {
        var books = await _unitOfWork.Books.GetByAuthorIdAsync(authorId);
        return Success(books.Where(b => b.IsActive).Select(ToDto).ToList());
    }

    public async Task<ResponseDto<PagedResultDto<BookDto>>> SearchAsync(BookFilterDto filter, int pageNumber, int pageSize)
    {
        var searchFilter = new BookSearchFilter
        {
            Name = filter.Name,
            AuthorName = filter.AuthorName,
            Language = filter.Language,
            Publisher = filter.Publisher,
            Condition = filter.Condition,
            YearFrom = filter.YearFrom,
            YearTo = filter.YearTo
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

    // NOVO: "meus livros" paginado, pra usar na página de Perfil.
    // Traduz o PagedResult<Book> do Data pro PagedResultDto<BookDto> da Application.
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
        if (dto.AuthorIds.Count == 0)
            return Failure<BookDto>("At least one author is required", 400);

        var authorIds = dto.AuthorIds.Distinct().ToList();
        var authors = (await _unitOfWork.Authors.FindAllAsync(a => authorIds.Contains(a.Id) && a.IsActive)).ToList();

        if (authors.Count != authorIds.Count)
            return Failure<BookDto>("One or more authors not found", 404);

        var book = new Book
        {
            Id = Guid.NewGuid(),
            UserId = dto.UserId,
            IsOwner = dto.IsOwner,
            Name = dto.Name,
            PublicationYear = dto.PublicationYear,
            Photos = dto.Photos,
            Language = dto.Language,
            Publisher = dto.Publisher,
            ISBN = dto.ISBN,
            Pages = dto.Pages,
            Edition = dto.Edition,
            EstimatedValue = dto.EstimatedValue,
            Condition = dto.Condition,
            Authors = authors,
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
        var book = await _unitOfWork.Books.GetByIdWithAuthorsAsync(dto.Id);

        if (book is null || !book.IsActive)
            return Failure<BookDto>("Book not found", 404);

        if (dto.AuthorIds.Count == 0)
            return Failure<BookDto>("At least one author is required", 400);

        var authorIds = dto.AuthorIds.Distinct().ToList();
        var authors = (await _unitOfWork.Authors.FindAllAsync(a => authorIds.Contains(a.Id) && a.IsActive)).ToList();

        if (authors.Count != authorIds.Count)
            return Failure<BookDto>("One or more authors not found", 404);

        book.IsOwner = dto.IsOwner;
        book.Name = dto.Name;
        book.PublicationYear = dto.PublicationYear;
        book.Photos = dto.Photos;
        book.Language = dto.Language;
        book.Publisher = dto.Publisher;
        book.ISBN = dto.ISBN;
        book.Pages = dto.Pages;
        book.Edition = dto.Edition;
        book.EstimatedValue = dto.EstimatedValue;
        book.Condition = dto.Condition;
        book.UpdatedAt = DateTimeOffset.UtcNow;

        book.Authors.Clear();
        foreach (var author in authors)
            book.Authors.Add(author);

        _unitOfWork.Books.Update(book);
        await _unitOfWork.SaveChangesAsync();

        return Success(ToDto(book), "Book updated successfully");
    }

    // Soft delete: só zera IsActive, não remove a linha (herdado de AbstractBase).
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
            AuthorIds = book.Authors.Select(a => a.Id).ToList(),
            AuthorNames = book.Authors.Select(a => a.Name).ToList(),
            IsOwner = book.IsOwner,
            Name = book.Name,
            PublicationYear = book.PublicationYear,
            Photos = book.Photos,
            Language = book.Language,
            Publisher = book.Publisher,
            ISBN = book.ISBN,
            Pages = book.Pages,
            Edition = book.Edition,
            EstimatedValue = book.EstimatedValue,
            Condition = book.Condition
        };
    }
}