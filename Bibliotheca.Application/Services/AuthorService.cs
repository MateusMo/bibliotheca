// Bibliotheca.Application/Services/AuthorService.cs
using Bibliotheca.Application.Dtos.Author;
using Bibliotheca.Application.Dtos.Common;
using Bibliotheca.Data.Uow;
using Bibliotheca.Domain.Domains;

namespace Bibliotheca.Application.Services;

public class AuthorService : BaseService, IAuthorService
{
    private readonly IUnitOfWork _unitOfWork;

    public AuthorService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseDto<AuthorDto>> GetByIdAsync(Guid id)
    {
        var author = await _unitOfWork.Authors.GetByIdAsync(id);

        if (author is null || !author.IsActive)
            return Failure<AuthorDto>("Author not found", 404);

        return Success(ToDto(author));
    }

    public async Task<ResponseDto<List<AuthorDto>>> GetAllAsync()
    {
        var authors = await _unitOfWork.Authors.FindAllAsync(x => x.IsActive);
        return Success(authors.Select(ToDto).ToList());
    }

    public async Task<ResponseDto<AuthorDto>> CreateAsync(CreateAuthorDto dto)
    {
        var author = new Author
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Photo = dto.Photo,
            BirthDay = dto.BirthDay,
            DeathDay = dto.DeathDay,
            Description = dto.Description,
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        await _unitOfWork.Authors.AddAsync(author);
        await _unitOfWork.SaveChangesAsync();

        return Success(ToDto(author), "Author created successfully", 201);
    }

    public async Task<ResponseDto<AuthorDto>> UpdateAsync(UpdateAuthorDto dto)
    {
        var author = await _unitOfWork.Authors.GetByIdAsync(dto.Id);

        if (author is null || !author.IsActive)
            return Failure<AuthorDto>("Author not found", 404);

        author.Name = dto.Name;
        author.Photo = dto.Photo;
        author.BirthDay = dto.BirthDay;
        author.DeathDay = dto.DeathDay;
        author.Description = dto.Description;
        author.UpdatedAt = DateTimeOffset.UtcNow;

        _unitOfWork.Authors.Update(author);
        await _unitOfWork.SaveChangesAsync();

        return Success(ToDto(author), "Author updated successfully");
    }

    public async Task<ResponseDto<bool>> DeleteAsync(Guid id)
    {
        var author = await _unitOfWork.Authors.GetByIdAsync(id);

        if (author is null || !author.IsActive)
            return Failure<bool>("Author not found", 404);

        author.IsActive = false;
        author.UpdatedAt = DateTimeOffset.UtcNow;

        _unitOfWork.Authors.Update(author);
        await _unitOfWork.SaveChangesAsync();

        return Success(true, "Author removed successfully");
    }

    private static AuthorDto ToDto(Author author)
    {
        return new AuthorDto
        {
            Id = author.Id,
            CreatedAt = author.CreatedAt,
            UpdatedAt = author.UpdatedAt,
            Name = author.Name,
            Photo = author.Photo,
            BirthDay = author.BirthDay,
            DeathDay = author.DeathDay,
            Description = author.Description
        };
    }
}