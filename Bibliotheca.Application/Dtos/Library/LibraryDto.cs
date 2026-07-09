using Bibliotheca.Application.Dtos.Common;

namespace Bibliotheca.Application.Dtos.Library;

public class LibraryDto : BaseDto
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int ProfileScore { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<LibraryBookItemDto> Books { get; set; } = [];
    public int BookCount => Books.Count;
}