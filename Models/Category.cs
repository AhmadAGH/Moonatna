namespace Moonatna.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string NameAr { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public string? IconClass { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
    }
}
