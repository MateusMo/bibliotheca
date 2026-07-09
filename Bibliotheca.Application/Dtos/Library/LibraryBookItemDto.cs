using Bibliotheca.Domain.Enums;

namespace Bibliotheca.Application.Dtos.Library;

public class LibraryBookItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public int PublicationYear { get; set; }
    public string[] Photos { get; set; } = [];
    public LanguageEnum LanguageEnum { get; set; }
    public BookConditionEnum ConditionEnum { get; set; }
    public long ViewCount { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}