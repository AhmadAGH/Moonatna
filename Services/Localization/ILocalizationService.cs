namespace Moonatna.Services.Localization;

public interface ILocalizationService
{
    Task<IReadOnlyDictionary<string, string>> GetStringsAsync();
}
