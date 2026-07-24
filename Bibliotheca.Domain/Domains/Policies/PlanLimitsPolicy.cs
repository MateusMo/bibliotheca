using Bibliotheca.Domain.Enums;

namespace Bibliotheca.Domain.Policies;

/// <summary>
/// Fonte única de verdade para os limites de cada plano.
/// Qualquer ajuste de limite ou novo plano deve ser feito apenas aqui.
/// </summary>
public static class PlanLimitsPolicy
{
    private const int FreeMaxPhotosPerBook = 4;
    private const int PaidMaxPhotosPerBook = 20;

    private const int FreeMaxLibraries = 3;
    private const int PaidMaxLibraries = 50;

    public static int MaxPhotosPerBook(this PlanTypeEnum plan) => plan switch
    {
        PlanTypeEnum.Paid => PaidMaxPhotosPerBook,
        _ => FreeMaxPhotosPerBook
    };

    public static int MaxLibraries(this PlanTypeEnum plan) => plan switch
    {
        PlanTypeEnum.Paid => PaidMaxLibraries,
        _ => FreeMaxLibraries
    };
}