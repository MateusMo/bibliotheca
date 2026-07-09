using Bibliotheca.Domain.Enums;

namespace Bibliotheca.Domain.Domains;

public class Book : AbstractBase
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

    public User User { get; set; } = null!;
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Library> Libraries { get; set; } = new List<Library>();
}