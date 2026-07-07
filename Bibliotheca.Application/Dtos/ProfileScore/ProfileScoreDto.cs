using Bibliotheca.Application.Dtos.Common;

namespace Bibliotheca.Application.Dtos.ProfileScore;

public class ProfileScoreDto : BaseDto
{
    public Guid ProfileId { get; set; }
    public int TotalBooksAverageYear { get; set; }
    public int TotalViews { get; set; }
    public int TotalYearsOnline { get; set; }
    public int TotalScore { get; set; }
}