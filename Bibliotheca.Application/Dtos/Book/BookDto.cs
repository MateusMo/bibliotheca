using Bibliotheca.Application.Dtos.Common;
using Bibliotheca.Domain.Enums;

namespace Bibliotheca.Application.Dtos.Book;

public class BookDto : BaseDto
{
    public Guid UserId { get; set; }
    public bool IsOwner { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int PublicationYear { get; set; }
    public string[] Photos { get; set; } = [];
    public LanguageEnum LanguageEnum { get; set; }
    public string Publisher { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
    public int Pages { get; set; }
    public decimal EstimatedValue { get; set; }
    public BookConditionEnum ConditionEnum { get; set; }
    public long ViewCount { get; set; }
}