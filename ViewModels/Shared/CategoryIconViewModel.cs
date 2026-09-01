using Moonatna.Models;

namespace Moonatna.ViewModels.Shared;

// Everything a view needs to draw one category icon, whichever kind it is.
// Rendered through Views/Shared/_CategoryIcon.cshtml so the three call sites
// — pantry card, shopping card, organize zone — can't drift apart.
public class CategoryIconViewModel
{
    public const string DefaultFallbackClass = "fa-solid fa-basket-shopping";

    public CategoryMediaType MediaType { get; set; } = CategoryMediaType.FontAwesome;
    public string? IconClass { get; set; }
    public string? IconPath { get; set; }

    // drawn when the category has no icon of its own
    public string FallbackClass { get; set; } = DefaultFallbackClass;

    // A local icon only wins when it actually has a path: a row flipped to
    // MediaType 2 before its file is in place still draws the Font Awesome
    // class rather than a broken image.
    public bool UsesLocalIcon =>
        MediaType == CategoryMediaType.LocalIcon && !string.IsNullOrWhiteSpace(IconPath);

    public string ResolvedClass =>
        string.IsNullOrWhiteSpace(IconClass) ? FallbackClass : IconClass;

    public static CategoryIconViewModel From(Category? category, string? fallbackClass = null)
    {
        var vm = new CategoryIconViewModel();
        if (fallbackClass is not null) vm.FallbackClass = fallbackClass;
        if (category is null) return vm;

        vm.MediaType = category.MediaType;
        vm.IconClass = category.IconClass;
        vm.IconPath = category.IconPath;
        return vm;
    }
}
