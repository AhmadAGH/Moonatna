using System.Globalization;
using Moonatna.Repositories.Lookups;

namespace Moonatna.Services.Localization;

public class LocalizationService : ILocalizationService
{
    private readonly ILookupsRepository _lookups;
    private IReadOnlyDictionary<string, string>? _strings;

    public LocalizationService(ILookupsRepository lookups) => _lookups = lookups;

    public async Task<IReadOnlyDictionary<string, string>> GetStringsAsync()
    {
        if (_strings is not null)
            return _strings;

        var entries = await _lookups.GetAllLocalizationsAsync();
        var isArabic = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ar";

        _strings = entries.ToDictionary(
            e => e.Key,
            e => isArabic ? e.ValueAr : e.ValueEn);

        return _strings;
    }
