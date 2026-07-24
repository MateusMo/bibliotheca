using Bibliotheca.Domain.Enums;

namespace Bibliotheca.Domain.Domains;

public class User : AbstractBase
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public PlanTypeEnum PlanType { get; set; } = PlanTypeEnum.Free;

    public Profile? Profile { get; set; }
    public ICollection<Book> Books { get; set; } = new List<Book>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Library> Libraries { get; set; } = new List<Library>();
}