using Bibliotheca.Domain.Enums;

namespace Bibliotheca.Application.Dtos.Book;

public class BookFilterDto
{
    // Busca livre: compara com Name, Author e ISBN simultaneamente.
    public string? SearchText { get; set; }

    public int? YearFrom { get; set; }
    public int? YearTo { get; set; }

    // Data em que o exemplar foi cadastrado na plataforma (Book.CreatedAt).
    public DateTime? AddedFrom { get; set; }
    public DateTime? AddedTo { get; set; }

    public int? PagesFrom { get; set; }
    public int? PagesTo { get; set; }

    public LanguageEnum? Language { get; set; }
    public BookConditionEnum? Condition { get; set; }

    public decimal? ValueFrom { get; set; }
    public decimal? ValueTo { get; set; }
    public BookSortOptionEnum SortBy { get; set; } = BookSortOptionEnum.RecentlyAdded;
}