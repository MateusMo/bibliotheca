using Bibliotheca.Application.Dtos.Common;
using Bibliotheca.Application.Dtos.ProfileScore;
using Bibliotheca.Data.Uow;
using Bibliotheca.Domain.Domains;

namespace Bibliotheca.Application.Services;

public class ProfileScoreService : BaseService, IProfileScoreService
{
    // Pesos e tetos de normalização do algoritmo de pontuação de perfil.
    // Cada componente é normalizado para uma escala de 0 a 100 antes de
    // entrar na média ponderada, para que nenhuma métrica domine as outras
    // apenas por ter uma ordem de grandeza maior (ex.: views vs. anos).
    //
    // Regra de negócio (ProfileScore):
    // - Antiguidade da coleção (peso 35%): quanto mais antiga a média de
    //   publicação dos livros IsOwner=true do usuário, maior a pontuação.
    //   Teto: 200 anos de "idade média" já vale nota máxima (100).
    // - Visualizações (peso 25%): soma do ViewCount dos livros IsOwner=true,
    //   em escala logarítmica (log10), para que perfis muito virais não
    //   dominem o ranking de forma desproporcional. Teto: 100.000 views
    //   somadas já vale nota máxima (100).
    // - Tempo de conta (peso 15%): anos desde a criação da conta. Teto: 10
    //   anos já vale nota máxima (100).
    // - Quantidade de livros IsOwner=true (peso 25%): tamanho da coleção
    //   "de posse comprovada". Teto: 300 livros já vale nota máxima (100).
    // O cálculo considera exclusivamente livros com IsOwner=true — livros
    // apenas desejados ou não confirmados como posse não contam para a nota.
    private const double AntiquityWeight = 0.35;
    private const double ViewsWeight = 0.25;
    private const double YearsOnlineWeight = 0.15;
    private const double BookCountWeight = 0.25;

    private const int AntiquityCapYears = 200;
    private const double ViewsCapLog = 5; // log10(100_000)
    private const int YearsOnlineCap = 10;
    private const int BookCountCap = 300;

    private readonly IUnitOfWork _unitOfWork;

    public ProfileScoreService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseDto<ProfileScoreDto>> GetByUserIdAsync(Guid userId)
    {
        var profile = await _unitOfWork.Profiles.GetByUserIdAsync(userId);
        if (profile is null || !profile.IsActive)
            return Failure<ProfileScoreDto>("Profile not found", 404);

        var score = await _unitOfWork.ProfileScores.GetByProfileIdAsync(profile.Id);
        if (score is null)
            return Failure<ProfileScoreDto>("Profile score not calculated yet", 404);

        return Success(ToDto(score));
    }

    public async Task<ResponseDto<bool>> RecalculateAsync(Guid userId)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user is null || !user.IsActive)
            return Failure<bool>("User not found", 404);

        var profile = await _unitOfWork.Profiles.GetByUserIdAsync(userId);
        if (profile is null || !profile.IsActive)
            return Failure<bool>("Profile not found", 404);

        var stats = await _unitOfWork.Books.GetOwnershipStatsByUserIdAsync(userId);

        var currentYear = DateTimeOffset.UtcNow.Year;
        var yearsOnline = Math.Max(0, currentYear - user.CreatedAt.Year);
        var antiquityYears = stats.OwnedBooksCount > 0 ? Math.Max(0, currentYear - stats.AveragePublicationYear) : 0;

        var antiquityScore = Math.Min(100d, antiquityYears / (double)AntiquityCapYears * 100d);
        var viewsScore = Math.Min(100d, Math.Log10(stats.TotalViews + 1) / ViewsCapLog * 100d);
        var yearsOnlineScore = Math.Min(100d, yearsOnline / (double)YearsOnlineCap * 100d);
        var bookCountScore = Math.Min(100d, stats.OwnedBooksCount / (double)BookCountCap * 100d);

        var totalScore = (antiquityScore * AntiquityWeight)
            + (viewsScore * ViewsWeight)
            + (yearsOnlineScore * YearsOnlineWeight)
            + (bookCountScore * BookCountWeight);

        var existing = await _unitOfWork.ProfileScores.GetByProfileIdAsync(profile.Id);
        var averageYear = stats.OwnedBooksCount > 0 ? stats.AveragePublicationYear : 0;
        var totalViews = (int)Math.Min(stats.TotalViews, int.MaxValue);

        if (existing is null)
        {
            var profileScore = new ProfileScore
            {
                Id = Guid.NewGuid(),
                ProfileId = profile.Id,
                TotalBooksAverageYear = averageYear,
                TotalViews = totalViews,
                TotalYearsOnline = yearsOnline,
                TotalScore = (int)Math.Round(totalScore),
                IsActive = true,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };

            await _unitOfWork.ProfileScores.AddAsync(profileScore);
        }
        else
        {
            existing.TotalBooksAverageYear = averageYear;
            existing.TotalViews = totalViews;
            existing.TotalYearsOnline = yearsOnline;
            existing.TotalScore = (int)Math.Round(totalScore);
            existing.UpdatedAt = DateTimeOffset.UtcNow;

            _unitOfWork.ProfileScores.Update(existing);
        }

        await _unitOfWork.SaveChangesAsync();

        return Success(true, "Profile score recalculated successfully");
    }

    private static ProfileScoreDto ToDto(ProfileScore score)
    {
        return new ProfileScoreDto
        {
            Id = score.Id,
            CreatedAt = score.CreatedAt,
            UpdatedAt = score.UpdatedAt,
            ProfileId = score.ProfileId,
            TotalBooksAverageYear = score.TotalBooksAverageYear,
            TotalViews = score.TotalViews,
            TotalYearsOnline = score.TotalYearsOnline,
            TotalScore = score.TotalScore
        };
    }
}