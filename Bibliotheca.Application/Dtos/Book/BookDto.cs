using Bibliotheca.Application.Dtos.Common;
using Bibliotheca.Domain.Enums;

namespace Bibliotheca.Application.Dtos.Book;

public class BookDto : BaseDto
{
    public Guid UserId { get; set; }
    public List<Guid> AuthorIds { get; set; } = [];
    public List<string> AuthorNames { get; set; } = [];
    public bool IsOwner { get; set; }
    public string Name { get; set; } = string.Empty;
    public int PublicationYear { get; set; }
    public string[] Photos { get; set; } = [];
    public string Language { get; set; } = string.Empty;
    public string Publisher { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
    public int Pages { get; set; }
    public string Edition { get; set; } = string.Empty;
    public decimal EstimatedValue { get; set; }
    public BookCondition Condition { get; set; }
}