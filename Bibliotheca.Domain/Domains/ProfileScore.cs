namespace Bibliotheca.Domain.Domains;

public class ProfileScore : AbstractBase
{
    public Guid ProfileId { get; set; }
    public int TotalBooksAverageYear { get; set; }
    public int TotalViews { get; set; }
    public int TotalYearsOnline { get; set; }
    public int TotalScore { get; set; }
}