using Bibliotheca.Domain.Enums;

namespace Bibliotheca.Application.Dtos.Book;

public class BookFilterDto
{
    public string? Name { get; set; }
    public string? AuthorName { get; set; }
    public string? Language { get; set; }
    public string? Publisher { get; set; }
    public BookCondition? Condition { get; set; }
    public int? YearFrom { get; set; }
    public int? YearTo { get; set; }
}