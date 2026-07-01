namespace Bibliotheca.Domain.Domains;

public class Profile : AbstractBase
{
    public Guid UserId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Contact { get; set; } = string.Empty;

    public User User { get; set; } = null!;
}