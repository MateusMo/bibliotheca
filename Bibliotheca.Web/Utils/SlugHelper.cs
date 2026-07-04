using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Bibliotheca.Web.Utils;

public static class SlugHelper
{
    public static string GenerateSlug(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        var normalized = input.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();
        foreach (var c in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }

        var slug = sb.ToString().Normalize(NormalizationForm.FormC).ToLowerInvariant();
        slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        slug = Regex.Replace(slug, @"\s+", "-").Trim('-');
        slug = Regex.Replace(slug, @"-+", "-");

        return slug.Length > 80 ? slug[..80].TrimEnd('-') : slug;
    }

    /// <summary>
    /// Slug canônico do livro: título + autor + ano + ISBN (quando existir).
    /// Ex.: "a-sombra-do-vento-machado-de-assis-1899-978651234567"
    /// </summary>
    public static string BuildBookSlug(
        string name,
        string author,
        int publicationYear,
        string? isbn = null)
    {
        var parts = new List<string> { name };

        if (!string.IsNullOrWhiteSpace(author))
            parts.Add(author);

        if (publicationYear > 0)
            parts.Add(publicationYear.ToString());

        var slug = GenerateSlug(string.Join(" ", parts));

        if (!string.IsNullOrWhiteSpace(isbn))
        {
            var isbnDigits = Regex.Replace(isbn, @"[^0-9Xx]", "");
            if (!string.IsNullOrEmpty(isbnDigits))
                slug = $"{slug}-{isbnDigits}";
        }

        return slug;
    }
}