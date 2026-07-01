using Bibliotheca.Domain.Enums;

namespace Bibliotheca.Domain.Domains;

public class Book : AbstractBase
{
    public Guid UserId { get; set; }
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

    public User User { get; set; } = null!;
    public ICollection<Author> Authors { get; set; } = new List<Author>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}