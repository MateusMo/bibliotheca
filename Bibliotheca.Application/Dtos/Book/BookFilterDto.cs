using Bibliotheca.Domain.Enums;

namespace Bibliotheca.Application.Dtos.Book;

public class BookFilterDto
{
    public string? Name { get; set; }
    public string? AuthorName { get; set; }
    public LanguageEnum? Language { get; set; }
    public string? Publisher { get; set; }
    public BookConditionEnum? Condition { get; set; }
    public int? YearFrom { get; set; }
    public int? YearTo { get; set; }
}