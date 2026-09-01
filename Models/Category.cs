namespace Moonatna.Models
{
    // How a category's icon is drawn — [Lookup].[Categories].[MediaType].
    public enum CategoryMediaType : byte
    {
        FontAwesome = 1,   // IconClass holds a Font Awesome class
        LocalIcon = 2      // IconPath holds a file under wwwroot (the designed SVGs)
    }

    public class Category
    {
        public int Id { get; set; }
        public string NameAr { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public string? IconClass { get; set; }
        public CategoryMediaType MediaType { get; set; } = CategoryMediaType.FontAwesome;
        public string? IconPath { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
    }
}
