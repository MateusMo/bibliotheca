namespace Bibliotheca.Application.Dtos.Author;

public class UpdateAuthorDto
{
    public Guid Id { get; set; }

    public string Photo { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public DateTime BirthDay { get; set; }

    public DateTime? DeathDay { get; set; }

    public string Description { get; set; } = string.Empty;
}