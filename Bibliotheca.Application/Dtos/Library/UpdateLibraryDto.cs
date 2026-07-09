namespace Bibliotheca.Application.Dtos.Library;

public class UpdateLibraryDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<Guid> BookIds { get; set; } = [];
}