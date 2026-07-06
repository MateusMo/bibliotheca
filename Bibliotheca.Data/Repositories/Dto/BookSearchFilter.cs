using Bibliotheca.Domain.Enums;

namespace Bibliotheca.Data.Repositories.Dto;

public class BookSearchFilter
{
    public string? SearchText { get; set; }

    public int? YearFrom { get; set; }
    public int? YearTo { get; set; }

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