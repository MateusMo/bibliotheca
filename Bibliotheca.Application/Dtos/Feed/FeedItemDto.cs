using Bibliotheca.Domain.Enums;

namespace Bibliotheca.Application.Dtos.Feed;

public enum FeedItemType
{
    Book = 1,
    Library = 2
}

/// <summary>
/// Item unificado do feed: representa um livro ou uma biblioteca.
/// Campos específicos de cada tipo ficam com valor padrão quando não se aplicam.
/// </summary>
public class FeedItemDto
{
    public FeedItemType Type { get; set; }
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int ProfileScore { get; set; }

    // Preenchido apenas quando Type == Book.
    public int PublicationYear { get; set; }
    public int PhotoCount { get; set; }
    public LanguageEnum? LanguageEnum { get; set; }
    public BookConditionEnum? ConditionEnum { get; set; }
    public long ViewCount { get; set; }

    // Preenchido apenas quando Type == Library.
    public int BookCount { get; set; }
}