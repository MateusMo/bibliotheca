namespace Bibliotheca.Data.Repositories.Dto;

/// <summary>
/// Estatísticas agregadas dos livros IsOwner=true de um usuário,
/// calculadas em uma única consulta para alimentar o ProfileScore
/// sem carregar as entidades inteiras.
/// </summary>
public class BookOwnershipStats
{
    public int OwnedBooksCount { get; set; }
    public int AveragePublicationYear { get; set; }
    public long TotalViews { get; set; }
}